using Microsoft.AspNetCore.Identity;

namespace BookMyMovies.Models
{
    public class BookingNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MoviePostingId { get; set; }
        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

        public IdentityUser User { get; set; }
        public MoviePosting MoviePosting { get; set; }
    }
}
