namespace BerrasBio.Models
{
    // Information about a movie.
    public class Movie
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Minutes { get; set; }
        public int AgeLimit { get; set; }
        public int PublishedYear { get; set; }
        public double Price { get; set; }
        public string? PhotoURL { get; set; }
    }
}

