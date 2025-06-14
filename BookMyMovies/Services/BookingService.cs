using BookMyMovies.Data;
using BookMyMovies.Helpers;
using BookMyMovies.Models;
using BookMyMovies.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;

namespace BookMyMovies.Services
{
    public class BookingService : IBookingService
    {
        private readonly EmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IRepository<MoviePosting> _repository;
        private readonly ApplicationDbContext _context;

        public BookingService(
            IRepository<MoviePosting> repository,
            EmailService emailService,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment env,
            ApplicationDbContext context)
        {
            _repository = repository;
            _emailService = emailService;
            _userManager = userManager;
            _env = env;
            _context = context;
        }

        public async Task<(bool success, string message, string? pdfUrl)> BookTicketsAsync(
            int movieId,
            List<string> selectedSeats,
            ClaimsPrincipal user,
            int popcornQty = 0,
            int coldDrinkQty = 0 )
        {
            var movie = await _repository.GetByIdAsync(movieId);
            if (movie == null) return (false, "Movie not found", null);

            var bookedSeats = string.IsNullOrEmpty(movie.SeatLayoutJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(movie.SeatLayoutJson);

            bookedSeats.AddRange(selectedSeats);
            movie.SeatLayoutJson = JsonSerializer.Serialize(bookedSeats);
            movie.SeatsBooked = bookedSeats.Count;


            var userId = _userManager.GetUserId(user);
            var identityUser = await _userManager.GetUserAsync(user);

            float ticketTotal = selectedSeats.Count * movie.Price;
            float popcornTotal = popcornQty * 80;       // ₹80 per popcorn
            float coldDrinkTotal = coldDrinkQty * 50;   // ₹50 per cold drink
            float totalAmount = ticketTotal + popcornTotal + coldDrinkTotal;

            var booking = new Booking
            {
                UserId = userId,
                MoviePostingId = movie.Id,
                BookingDate = DateTime.UtcNow,
                PaymentStatus = "Paid",
                SeatNumbers = string.Join(", ", selectedSeats),
                SeatsBooked = selectedSeats.Count,
                PopcornQty = popcornQty,
                ColdDrinkQty = coldDrinkQty,
                TotalAmount = totalAmount
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            await ProcessPostBookingAsync(booking, identityUser, movie, selectedSeats, popcornQty, coldDrinkQty);

            await _context.SaveChangesAsync();
            await _repository.UpdateAsync(movie);

            return (true, $"Successfully booked {selectedSeats.Count} seats", booking.PdfPath);
        }

        public async Task ProcessPostBookingAsync(
            Booking booking,
            IdentityUser user,
            MoviePosting movie,
            List<string> selectedSeats,
            int popcornQty = 0,
            int coldDrinkQty = 0)
        {
            var email = await _userManager.GetEmailAsync(user);
            int seatCount = selectedSeats.Count;
            float ticketAmount = seatCount * movie.Price;
            float popcornAmount = popcornQty * 80;
            float coldDrinkAmount = coldDrinkQty * 50;
            float totalAmount = ticketAmount + popcornAmount + coldDrinkAmount;
            var seats = string.Join(", ", selectedSeats);

            string body = $@"
                <h2>Ticket Confirmation</h2>
                <p>Hi {user.UserName},</p>
                <p>You have successfully booked <strong>{seatCount}</strong> ticket(s) for the movie: <strong>{movie.Title}</strong>.</p>
                <p><strong>Seats:</strong> {seats}</p>
                <p><strong>Snacks Ordered:</strong><br />
                🍿 Popcorn: {popcornQty} × ₹80 = ₹{popcornAmount}<br />
                🥤 Cold Drink: {coldDrinkQty} × ₹50 = ₹{coldDrinkAmount}</p>
                <p><strong>Total Amount Paid:</strong> ₹{totalAmount}</p>
                <p><strong>Location:</strong> {movie.Theater}, {movie.Location}</p>
                <p><em>Thank you for booking with BookMyMovies!</em></p>";

            await _emailService.SendEmailAsync(email, "🎟 Ticket Booked - BookMyMovies", body);

            var qrData = $"User: {user.UserName}, Movie: {movie.Title}, Seats: {seats}";
            var qrCodeImage = QRHelper.GenerateQrCode(qrData);

            var pdfBytes = PDFHelper.GenerateBookingPdf(user.UserName, movie.Title, seats, movie.Location, qrCodeImage);
            var pdfFileName = $"Ticket_{booking.Id}.pdf";

            var pdfPath = Path.Combine(_env.WebRootPath, "pdfs", pdfFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(pdfPath)!);
            File.WriteAllBytes(pdfPath, pdfBytes);

            booking.PdfPath = "/pdfs/" + pdfFileName;
        }
    }
}
