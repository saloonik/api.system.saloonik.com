using beautysalon.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace beautysalon.Database.Extensions
{
    public static class ModelExtensions
    {
        public static void ConfigureReservationService (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Services)
                .WithMany(s => s.Reservations)
                .UsingEntity<Dictionary<string, object>>(
                    "ReservationService",
                    j => j.HasOne<Service>().WithMany().HasForeignKey("ServiceId"),
                    j => j.HasOne<Reservation>().WithMany().HasForeignKey("ReservationId"));
        }

        public static void ConfigureCompanyServices(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Service>()
                .HasOne(s => s.Company)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
