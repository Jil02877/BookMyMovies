using BookMyMovies.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookMyMovies.Controllers
{
    [Authorize(Roles = "Admin,Employer")]
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> MovieBusiness()
        {
            var revenueData = await _context.Bookings
                .Include(b => b.MoviePosting)
                .GroupBy(b => b.MoviePosting.Title)
                .Select(g => new
                {
                    Movie = g.Key,
                    Revenue = g.Sum(b => b.TotalAmount)
                }).ToListAsync();

            var ticketData = await _context.Bookings
                .Include(b => b.MoviePosting)
                .GroupBy(b => b.MoviePosting.Title)
                .Select(g => new
                {
                    Movie = g.Key,
                    Tickets = g.Sum(b => b.SeatsBooked)
                }).ToListAsync();

            ViewBag.RevenueData = revenueData;
            ViewBag.TicketData = ticketData;
            return View();
        }
    }
}
