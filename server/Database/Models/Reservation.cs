using System.ComponentModel.DataAnnotations;

namespace beautysalon.Database.Models
{
    public class Reservation
    {
        [Key]
        public Guid ReservationId { get; set; }

        [Required]
        public ICollection<Service> Services { get; set; } = new List<Service>();

        [Required]
        public Client Client { get; set; }

        [Required]
        public Company Company { get; set; }

        public decimal TotalPrice
        {
            get
            {
                return Services.Sum(service => service.Price);
            }
        }
    }
}
