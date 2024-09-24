using System.ComponentModel.DataAnnotations;

namespace MovieApi
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; } = "";

        public DateOnly ReleaseDate { get; set; }

        [Required]
        public float Rating { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}
