using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace beautysalon.AuthContracts
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "Email length can't be more than 100.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        [NotNull]
        public required string Email { get; set; }

        [Required]
        [NotNull]
        [PasswordPropertyText]
        [MinLength(6, ErrorMessage = "Password length can't be less than 6.")]
        public required string Password { get; set; }
    }
}
