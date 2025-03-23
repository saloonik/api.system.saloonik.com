namespace beautysalon.Database.Models
{
    public class Service
    {
        public Guid ServiceId { get; set; } = Guid.NewGuid();
        public required string ServiceName { get; set; }
        public required string ServiceDescription { get; set; }
        public required string ServiceCategory { get; set; }
        public decimal Price { get; set; }
        public Guid CompanyID { get; set; }
        public Company Company { get; set; }
        public List<Reservation>? Reservations { get; set; }
        
    }
}
