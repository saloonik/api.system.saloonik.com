namespace beautysalon.Database.Models
{
    public class Company
    {
        public Guid CompanyId { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Nip { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public virtual ICollection<Client>? ApplicationUsers { get; set; }
        public virtual ICollection<Staff>? Staff {  get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
        public virtual ICollection<Service>? Services { get; set; }

    }
}
