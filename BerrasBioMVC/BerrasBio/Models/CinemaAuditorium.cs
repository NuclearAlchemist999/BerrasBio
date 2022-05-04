namespace BerrasBio.Models
{
    // Here are the properties for the salons in the cinema. They have different sizes which will
    // reflect in the number of seats.
    public class CinemaAuditorium
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int NumberOfSeats { get; set; }
       
    }
}
