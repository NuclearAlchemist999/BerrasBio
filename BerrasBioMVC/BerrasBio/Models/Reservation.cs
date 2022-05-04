namespace BerrasBio.Models
{
    // When a customer is booking a ticket a reservation will be added in the database.
    public class Reservation
    {
        public int Id { get; set; }
        public int ScheduledMovieId { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public ScheduledMovie? ScheduledMovie { get; set; }
        public int NumberOfseats { get; set; }
        public double Price { get; set; }

    }
}
