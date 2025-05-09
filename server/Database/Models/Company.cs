using System.ComponentModel.DataAnnotations;

namespace beautysalon.Database.Models
{
    public class Company
    {
        [Key]
        public Guid CompanyId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Nip { get; set; }

        [Required]
        [MaxLength(200)]
        public string Street { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string State { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        public virtual ICollection<Client>? ApplicationUsers { get; set; }
        public virtual ICollection<Staff>? Staff { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
        public virtual ICollection<Service>? Services { get; set; }
    }
}
