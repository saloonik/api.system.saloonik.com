using System.ComponentModel.DataAnnotations;

namespace beautysalon.Database.Models
{
    public class Service
    {
        [Key]
        public Guid ServiceId { get; set; } 

        [Required]
        [StringLength(100, ErrorMessage = "Service name cannot exceed 100 characters.")]
        public string ServiceName { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Service description cannot exceed 500 characters.")]
        public string ServiceDescription { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Service category cannot exceed 50 characters.")]
        public string ServiceCategory { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required]
        public Company Company { get; set; }

        public List<Reservation>? Reservations { get; set; }
    }
}
