using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace beautysalon.Database.Models
{
    public class Staff : IdentityUser<Guid>
    {
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value.")]
        public decimal? Salary { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary bonus must be a positive value.")]
        public decimal? SalaryBonus { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? DOB { get; set; }

        [Required]
        public Company Company { get; set; }

        public List<Reservation>? Reservations { get; set; }
    }
}
