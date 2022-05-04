namespace BerrasBio.Models
{
    // The movies that will be scheduled and the information needed about when and where and
    // how many seats there are left.
    public class ScheduledMovie
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int AuditoriumId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public CinemaAuditorium? Auditorium { get; set; }
        public Movie? Movie { get; set; }
        public int SeatsLeft { get; set; }
        
    }
}
