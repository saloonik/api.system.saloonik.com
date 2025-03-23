using Microsoft.AspNetCore.Identity;

namespace beautysalon.Database.Models
{
    public class Staff : IdentityUser<Guid>
    {
        public decimal? Salary { get; set; }
        public decimal? SalaryBonus { get; set; }
        public DateTime? DOB { get; set; }
        public Guid CompanyId { get; set; }
        public Company? Company { get; set; }
        public List<Reservation>? Reservations { get; set; }
    }
}
