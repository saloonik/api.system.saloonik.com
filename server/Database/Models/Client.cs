namespace beautysalon.Database.Models
{
    public class Client
    {
        public Guid ClientId { get; set; } = Guid.NewGuid();
        public required string FirstName { get; set; } 
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Street { get; set; } 
        public string? City { get; set; } 
        public string? State { get; set; }
        public string? PostalCode { get; set; } 
        public string? Country { get; set; }
        public required Guid CompanyId { get; set; }
        public required Company Company { get; set; }
        public Guid ReservationsID { get; set; }
        public List<Reservation>? Reservations { get; set; }
    }
}
