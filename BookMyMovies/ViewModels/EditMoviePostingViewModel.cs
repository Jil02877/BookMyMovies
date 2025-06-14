using System.ComponentModel.DataAnnotations;
namespace BookMyMovies.ViewModels
{
    public class EditMoviePostingViewModel
    {

        public int Id { get; set; }

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

        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }

        public int SeatsBooked { get; set; }
    }
}
