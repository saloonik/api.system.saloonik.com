using FluentValidation;

namespace beautysalon.Logic.DTOs
{
    public class ClientDTO
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }

    public class ClientDTOValidator : AbstractValidator<ClientDTO>
    {
        public ClientDTOValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Imię jest wymagane.")
                .MaximumLength(50).WithMessage("Imię nie może przekroczyć 50 znaków.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Nazwisko jest wymagane.")
                .MaximumLength(50).WithMessage("Nazwisko nie może przekroczyć 50 znaków.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Adres e-mail jest wymagany.")
                .EmailAddress().WithMessage("Niepoprawny format adresu e-mail.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Numer telefonu jest wymagany.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Niepoprawny format numeru telefonu.");

            RuleFor(x => x.Street)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Street))
                .WithMessage("Ulica nie może przekroczyć 100 znaków.");

            RuleFor(x => x.City)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage("Miasto nie może przekroczyć 50 znaków.");

            RuleFor(x => x.State)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.State))
                .WithMessage("Województwo nie może przekroczyć 50 znaków.");

            RuleFor(x => x.PostalCode)
                .MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.PostalCode))
                .WithMessage("Kod pocztowy nie może przekroczyć 20 znaków.");

            RuleFor(x => x.Country)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Country))
                .WithMessage("Kraj nie może przekroczyć 50 znaków.");
        }
    }
}
