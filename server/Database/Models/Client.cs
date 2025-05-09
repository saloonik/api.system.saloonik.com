using System.ComponentModel.DataAnnotations;

namespace beautysalon.Database.Models
{
    public class Client
    {
        [Key]
        public Guid ClientId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?\d{7,15}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [MaxLength(100, ErrorMessage = "Street cannot exceed 100 characters.")]
        public string? Street { get; set; }

        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string? City { get; set; }

        [MaxLength(50, ErrorMessage = "State cannot exceed 50 characters.")]
        public string? State { get; set; }

        [MaxLength(20, ErrorMessage = "Postal code cannot exceed 20 characters.")]
        public string? PostalCode { get; set; }

        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string? Country { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Company Company { get; set; }

        public Guid ReservationsID { get; set; }

        public List<Reservation>? Reservations { get; set; }
    }
}
