namespace beautysalon.Database.Models
{
    public class Reservation
    {
        public Guid ReservationId { get; set; } = Guid.NewGuid();
        public required List<Service> Services { get; set; }
        public Guid ClientId { get; set; }
        public required Client Client { get; set; }
        public Guid CompanyID { get; set; }
        public required Company Company { get; set; }

        public decimal TotalPrice
        {
            get
            {
                return Services.Sum(service => service.Price);
            }
        }
    }
}
