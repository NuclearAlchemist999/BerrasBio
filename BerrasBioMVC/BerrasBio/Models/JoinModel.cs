namespace BerrasBio.Models
{
    // A model that is used when joining tables are needed.
    public class JoinModel 
    {
        public int ScheduleId { get; set; }
        public double Price { get; set; }   
        public int SeatsLeft { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? AuditoriumName { get; set; }
        public string? Title { get; set; }
        
        
    }
}
