using BookMyMovies.Models;
using BookMyMovies.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookMyMovies.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BookMyMovies.Constants;
using Microsoft.AspNetCore.Authorization;
namespace BookMyMovies.Controllers
{
    [Authorize]
    public class MoviePostingsController : Controller
    {
        private readonly IRepository<MoviePosting> _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public MoviePostingsController(IRepository<MoviePosting> repository,UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
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
            var movie = await _repository.GetByIdAsync(id);
            if (movie == null) return NotFound();

            var bookedSeats = string.IsNullOrEmpty(movie.SeatLayoutJson) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(movie.SeatLayoutJson);

            bookedSeats.AddRange(selectedSeats);

            bookedSeats = bookedSeats.Distinct().ToList();

            movie.SeatLayoutJson = System.Text.Json.JsonSerializer.Serialize(bookedSeats);

            movie.SeatsAvailable = movie.TotalSeats - bookedSeats.Count;
            movie.IsSeatAvailable = movie.SeatsAvailable > 0;

            int seatCount = selectedSeats.Count;
            float totalAmount = seatCount * movie.Price;

            // You can pass this to a View, send it in a confirmation, etc.
            TempData["BookingMessage"] = $"Successfully booked {seatCount} seat(s) for ₹{totalAmount}";


            await _repository.UpdateAsync(movie);
            return RedirectToAction(nameof(Index));
        }
    }
}
