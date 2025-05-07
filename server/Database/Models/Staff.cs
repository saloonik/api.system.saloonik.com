using Microsoft.AspNetCore.Identity;

namespace beautysalon.Database.Models
{
    public class Staff : IdentityUser<Guid>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public decimal? Salary { get; set; }
        public decimal? SalaryBonus { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? DOB { get; set; }
        public Guid CompanyId { get; set; }
        public required Company Company { get; set; }
        public List<Reservation>? Reservations { get; set; }
    }
}
