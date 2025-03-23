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
    }
}
