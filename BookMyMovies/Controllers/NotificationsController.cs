using BookMyMovies.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookMyMovies.Models;
namespace BookMyMovies.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int movieId)
        {
            var userId = _userManager.GetUserId(User);
            var alreadySubscribed = await _context.BookingNotifications
                .AnyAsync(n => n.MoviePostingId == movieId && n.UserId == userId);

            if (!alreadySubscribed)
            {
                _context.BookingNotifications.Add(new BookingNotification
                {
                    MoviePostingId = movieId,
                    UserId = userId
                });
                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "You will be notified when booking opens!";
            return RedirectToAction("Details", "MoviePostings", new { id = movieId });
        }
    }
}
