using System.ComponentModel.DataAnnotations;
namespace BookMyMovies.ViewModels
{
    public class CreateMoviePostingViewModel
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
        public int TotalSeats { get; set; }

        [Required]
        public float Price { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile ImageFile { get; set; }
    }
}
