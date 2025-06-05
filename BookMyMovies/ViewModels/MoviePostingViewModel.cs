using System.ComponentModel.DataAnnotations;
namespace BookMyMovies.ViewModels
{
    public class MoviePostingViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Theater { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public int SeatsAvailable { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        public string? SeatLayoutJson { get; set; }

        [Required]
        public float Price { get; set; } = 0.0f;

        [Required]
        public IFormFile ImageFile { get; set; }
        
    }
}
