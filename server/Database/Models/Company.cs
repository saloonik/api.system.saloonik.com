namespace beautysalon.Database.Models
{
    public class Company
    {
        public Guid CompanyId { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Nip { get; set; }
        public required string Street { get; set; }
        public required string StreetNumber { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
        public virtual ICollection<Client>? ApplicationUsers { get; set; }
        public virtual ICollection<Staff>? Staff {  get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
        public virtual ICollection<Service>? Services { get; set; }

    }
}
