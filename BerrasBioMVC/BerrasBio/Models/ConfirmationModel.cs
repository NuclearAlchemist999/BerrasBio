namespace BerrasBio.Models
{
    // A model for the information needed on the confirmation page.
    public class ConfirmationModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int ReservationId { get; set; }
        public double Price { get; set; }
        public int NumberOfSeats { get; set; }
        public string? Movie { get; set; }
        public string? AuditoriumName { get; set; }
        public DateTime Start { get; set; }

    }
}
