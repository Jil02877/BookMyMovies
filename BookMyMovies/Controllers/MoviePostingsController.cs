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
            if(User.IsInRole(Roles.Employer))
            {
                var allMoviePostings = await _repository.GetAllAsync();
                var userId = _userManager.GetUserId(User);
                var filterdMoviePostings = allMoviePostings.Where(mp => mp.UserId == userId);
                return View(filterdMoviePostings);
            }
            var moviePostings = await _repository.GetAllAsync();
            return View(moviePostings);
        }
        [Authorize(Roles = "Admin,Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create(MoviePostingViewModel moviePostingVm)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (moviePostingVm.ImageFile != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + moviePostingVm.ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await moviePostingVm.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var moviePosting = new MoviePosting
                {
                    Title = moviePostingVm.Title,
                    Description = moviePostingVm.Description,
                    Theater = moviePostingVm.Theater,
                    Location = moviePostingVm.Location,
                    SeatsAvailable = moviePostingVm.SeatsAvailable,
                    TotalSeats = moviePostingVm.TotalSeats,
                    UserId = _userManager.GetUserId(User),
                    ImageUrl = uniqueFileName != null ? "/images/" + uniqueFileName : null,
                    Price = moviePostingVm.Price
                };

                await _repository.AddAsync(moviePosting);
                return RedirectToAction(nameof(Index));
            }

            return View(moviePostingVm);
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
        public async Task<IActionResult> BookTicket(int id,List<string> selectedSeats)
        {
            var result = await _bookingService.BookTicketsAsync(id, selectedSeats, User);
            if (!result.success)
            {
                TempData["Error"] = result.message;
                return RedirectToAction(nameof(Index));
            }

            TempData["BookingMessage"] = result.message;
            TempData["PDFTicketPath"] = result.pdfUrl;

            return RedirectToAction(nameof(Index));
        }


    }
}
