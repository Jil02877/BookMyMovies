using BookMyMovies.Models;
namespace BookMyMovies.ViewModels
{
    public class MovieListingWithBookingViewModel
    {
        public IEnumerable<MoviePosting> Movies { get; set; }
        public List<int> MovieIdsUserHasBooked { get; set; } = new(); // MoviePostingIds user has bookings for
    }
}
