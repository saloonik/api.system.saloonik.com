using FluentValidation;
using beautysalon.Logic.Services.Validators;

namespace beautysalon.Contracts
{
    public class RegisterRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string CompanyName { get; set; }
        public required string Street { get; set; }
        public required string Headquarters { get; set; }
        public required string Regon { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Nip { get; set; }
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Imię i nazwisko jest wymagane")
                .MinimumLength(3).WithMessage("Imię i nazwisko musi mieć co najmniej 3 znaki");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email jest wymagany")
                .EmailAddress().WithMessage("Niepoprawny format adresu email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Hasło jest wymagane")
                .MinimumLength(8).WithMessage("Hasło musi mieć co najmniej 8 znaków")
                .Matches(@"[A-Z]+").WithMessage("Hasło musi zawierać przynajmniej jedną wielką literę")
                .Matches(@"[a-z]+").WithMessage("Hasło musi zawierać przynajmniej jedną małą literę")
                .Matches(@"\d+").WithMessage("Hasło musi zawierać przynajmniej jedną cyfrę")
                .Matches(@"[\W_]+").WithMessage("Hasło musi zawierać przynajmniej jeden znak specjalny");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Nazwa firmy jest wymagana");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Ulica jest wymagana");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Kraj jest wymagany");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Miasto jest wymagane");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Kod pocztowy jest wymagany")
                .Matches(@"^\d{2}-\d{3}$").WithMessage("Niepoprawny format kodu pocztowego (np. 00-000)");

            RuleFor(x => x.Nip)
                .NotEmpty().WithMessage("NIP jest wymagany")
                .MustAsync(async (model, nip, cancellationToken) => await PolishIDNumbersValidation.IsValidNIPAsync(nip, cancellationToken))
                .WithMessage("NIP jest niepoprawny");
            RuleFor(x => x.Regon)
                .NotEmpty().WithMessage("Regon jest wymagany")
                .MustAsync(async (model, regon, cancellationToken) => await PolishIDNumbersValidation.IsValidREGONAsync(regon, cancellationToken))
                .WithMessage("Regon jest niepoprawny");

            RuleFor(x => x.Headquarters)
                .NotEmpty().WithMessage("Siedziba Firmy jest wymagana");

        }
    }
}
