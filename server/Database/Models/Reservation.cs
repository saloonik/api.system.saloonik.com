using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace beautysalon.Database.Models
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ReservationId { get; set; }

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
