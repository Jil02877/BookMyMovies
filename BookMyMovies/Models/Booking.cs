using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookMyMovies.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        [Required]
        public int MoviePostingId { get; set; }

        [ForeignKey(nameof(MoviePostingId))]
        public MoviePosting MoviePosting { get; set; }

     
        public int SeatsBooked { get; set; }

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public string PaymentStatus { get; set; } = "Pending"; // or "Paid"

        public string? SeatNumbers { get; set; } // Optional: store "A1,A2,A3"

        public string? PdfPath { get; set; }
    }
}
