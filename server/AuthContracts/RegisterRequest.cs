﻿namespace beautysalon.Contracts
{
    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string CompanyName { get; set; }
        public required string CompanyNIP{ get; set; }
    }
}
