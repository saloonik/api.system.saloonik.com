using beautysalon.Database.Extensions;
using beautysalon.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace beautysalon.Database
{
    public class DatabaseContext : IdentityDbContext<Staff,IdentityRole<Guid>, Guid>
    {
        public DatabaseContext (DbContextOptions<DatabaseContext> options):base(options)
        { } 
        public DbSet<Company> Companies { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ApplicationRole> applicationRoles { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelExtensions.ConfigureReservationService (modelBuilder);
        }

    }
}
