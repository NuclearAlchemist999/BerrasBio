namespace BerrasBio.Models
{
    // Information about a customer. Mail is the most important one since it serves as an identifier.
    public class Customer
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Mail { get; set; }
    }
}
