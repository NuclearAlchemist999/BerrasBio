using BerrasBio.Models;
using Microsoft.EntityFrameworkCore;

namespace BerrasBio.Data
{
    // Code first.
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DataContext()
        {

        }

        public DbSet<CinemaAuditorium> CinemaAuditoriums { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ScheduledMovie> ScheduledMovies { get; set; }
     
       
     
        
        
    }
}
