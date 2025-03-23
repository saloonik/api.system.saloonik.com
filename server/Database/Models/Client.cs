namespace beautysalon.Database.Models
{
    public class Client
    {
        public Guid ClientId { get; set; } = Guid.NewGuid();
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Street { get; set; } 
        public string? City { get; set; } 
        public string? State { get; set; }
        public string? PostalCode { get; set; } 
        public string? Country { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
        public List<Reservation>? Reservations { get; set; }
    }
}
