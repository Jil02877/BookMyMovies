using BookMyMovies.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BookMyMovies.Services
{
    public interface IBookingService
    {
        Task ProcessPostBookingAsync(Booking booking, IdentityUser user, MoviePosting movie, List<string> selectedSeats, int popcornQty, int coldDrinkQty);
        Task<(bool success, string message, string? pdfUrl)> BookTicketsAsync(int movieId, List<string> selectedSeats, ClaimsPrincipal user, int popcornQty, int coldDrinkQty );
        Task<(bool success, string message, string? pdfUrl)> EditBookingAsync(int movieId, List<string> seatNumbers, ClaimsPrincipal user, int popcornQty, int coldDrinkQty);

    }
}
