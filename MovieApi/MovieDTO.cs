using System.ComponentModel.DataAnnotations;

namespace MovieApi
{
    public class MovieDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }
        public DateOnly ReleaseDate { get; set; }

        [Required]
        public float Rating { get; set; }

        public MovieDTO() { }

        public MovieDTO(Movie movie) =>
            (Id,  Title, ReleaseDate, Rating) 
            = (movie.Id, movie.Title, movie.ReleaseDate, movie.Rating);
    }
}
