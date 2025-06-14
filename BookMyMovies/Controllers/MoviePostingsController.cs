using BookMyMovies.Constants;
using BookMyMovies.Data;
using BookMyMovies.Helpers;
using BookMyMovies.Models;
using BookMyMovies.Repositories;
using BookMyMovies.Services;
using BookMyMovies.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Mono.TextTemplating;
namespace BookMyMovies.Controllers
{
    [Authorize]
    public class MoviePostingsController : Controller
    {
        private readonly IRepository<MoviePosting> _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IBookingService _bookingService;
        public MoviePostingsController(IRepository<MoviePosting> repository,UserManager<IdentityUser> userManager,EmailService emailService, ApplicationDbContext context,IBookingService bookingService)
        {
            _repository = repository;
            _userManager = userManager;
            _emailService = emailService;
            _context = context;
            _bookingService = bookingService;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if(User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Employer)) 
            {
                var allMoviePostings = await _repository.GetAllAsync();
                var userId = _userManager.GetUserId(User);
                var filteredMoviePostings = allMoviePostings
                    .Where(mp => mp.UserId == userId)
                    .OrderByDescending(mp => mp.PostedDate);

                return View(filteredMoviePostings);

            }
            if (User.IsInRole(Roles.User))
            {
                var allMoviePostings = await _repository.GetAllAsync();
                var sorted = allMoviePostings.OrderByDescending(mp => mp.PostedDate); 
                return View(sorted);
            }

            var latest3Movies = await _repository.GetAllAsync();
            var result = latest3Movies
                .OrderByDescending(mp => mp.PostedDate)
                .Take(3);

            return View(result);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _repository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [Authorize(Roles = "Admin,Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create(CreateMoviePostingViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            string uniqueFileName = null;

            if (vm.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                uniqueFileName = Guid.NewGuid() + "_" + vm.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(fs);
                }
            }

            var movie = new MoviePosting
            {
                Title = vm.Title,
                Description = vm.Description,
                Theater = vm.Theater,
                Location = vm.Location,
                SeatsBooked = 0,
                TotalSeats = vm.TotalSeats,
                Price = vm.Price,
                ImageUrl = "/images/" + uniqueFileName,
                UserId = _userManager.GetUserId(User),
            };

            await _repository.AddAsync(movie);
            return RedirectToAction(nameof(Index));
        }


        [HttpDelete]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Delete(int id)
        {
            var moviePosting = await _repository.GetByIdAsync(id);
            if(moviePosting == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            if(User.IsInRole(Roles.Admin) == false && moviePosting.UserId != userId)
            {
                return Forbid();
            }
            await _repository.DeleteAsync(id);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = Roles.User)]
        public async Task<IActionResult> BookTicket(int id)
        {
            var movie = await _repository.GetByIdAsync(id);
            if (movie == null) return NotFound();

            // Deserialize booked seats
            var bookedSeats = string.IsNullOrEmpty(movie.SeatLayoutJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(movie.SeatLayoutJson);

            ViewBag.BookedSeats = bookedSeats;

            return View(movie);
        }

        [HttpPost]
        [Authorize(Roles = Roles.User)]
        public async Task<IActionResult> BookTicket(int id,List<string> selectedSeats, int popcornQty = 0, int coldDrinkQty = 0)
        {
            var result = await _bookingService.BookTicketsAsync(id, selectedSeats, User, popcornQty, coldDrinkQty);
            if (!result.success)
            {
                TempData["Error"] = result.message;
                return RedirectToAction(nameof(Index));
            }

            TempData["BookingMessage"] = result.message;
            TempData["PDFTicketPath"] = result.pdfUrl;

            return RedirectToAction(nameof(BookTicket), new { id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _repository.GetByIdAsync(id);
            if (movie == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (!User.IsInRole(Roles.Admin) && movie.UserId != userId) return Forbid();

            var vm = new EditMoviePostingViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Theater = movie.Theater,
                Location = movie.Location,
                TotalSeats = movie.TotalSeats,
                Price = movie.Price,
                ExistingImageUrl = movie.ImageUrl,
                SeatsBooked = movie.TotalSeats - movie.SeatsAvailable
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Edit(int id, EditMoviePostingViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var existingMovie = await _repository.GetByIdAsync(id);
                vm.ExistingImageUrl = existingMovie?.ImageUrl;
                return View(vm);
            }

            var movie = await _repository.GetByIdAsync(id);
            if (movie == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (!User.IsInRole(Roles.Admin) && movie.UserId != userId) return Forbid();

            movie.Title = vm.Title;
            movie.Description = vm.Description;
            movie.Theater = vm.Theater;
            movie.Location = vm.Location;
            int bookedSeats = movie.SeatsBooked;
            movie.TotalSeats = vm.TotalSeats;
            movie.SeatsBooked = bookedSeats;
            movie.Price = vm.Price;

            if (vm.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var uniqueFileName = Guid.NewGuid() + "_" + vm.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(fs);
                }

                movie.ImageUrl = "/images/" + uniqueFileName;
            }

            await _repository.UpdateAsync(movie);
            return RedirectToAction(nameof(Index));
        }


    }
}
