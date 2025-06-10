using BookMyMovies.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BookMyMovies.Services
{
    public interface IBookingService
    {
        Task ProcessPostBookingAsync(Booking booking, IdentityUser user, MoviePosting movie, List<string> selectedSeats);
        Task<(bool success, string message, string? pdfUrl)> BookTicketsAsync(int movieId, List<string> selectedSeats, ClaimsPrincipal user);
    }
}
