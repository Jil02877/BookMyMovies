using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookMyMovies.Models
{
    public class MoviePosting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ImageUrl { get; set; }  // Save image path here

        [NotMapped]  //  Tells EF to ignore this property
        public IFormFile ImageFile { get; set; }  // For upload only


        [Required]
        public string Description { get; set; }

        [Required]
        public string Theater { get; set; }

        [Required]
        public string Location { get; set; }

        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; }

        public int SeatsBooked { get; set; } = 0;


        [Required]
        public int TotalSeats { get; set; }

        [NotMapped]
        public int SeatsAvailable => TotalSeats - SeatsBooked; 

        [NotMapped]
        public bool IsSeatAvailable => SeatsAvailable > 0; 

        public string? SeatLayoutJson { get; set; }

        public float Price { get; set; } = 0.0f;

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
