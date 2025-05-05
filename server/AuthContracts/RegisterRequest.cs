namespace beautysalon.Contracts
{
    public class RegisterRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string CompanyName { get; set; }
        public required string Street { get; set; }
        public required string StreetNumber { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string CompanyNIP{ get; set; }
    }
}
