using FluentValidation;

namespace beautysalon.AuthContracts
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email jest wymagany.")
                .EmailAddress().WithMessage("Niepoprawny format emaila.")
                .MaximumLength(100).WithMessage("Email nie może przekroczyć 100 znaków.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Hasło jest wymagane.")
                .MinimumLength(6).WithMessage("Hasło musi mieć co najmniej 6 znaków.")
                .Matches(@"[A-Z]").WithMessage("Hasło musi zawierać co najmniej jedną wielką literę.")
                .Matches(@"[a-z]").WithMessage("Hasło musi zawierać co najmniej jedną małą literę.")
                .Matches(@"[0-9]").WithMessage("Hasło musi zawierać co najmniej jedną cyfrę.")
                .Matches(@"[\W]").WithMessage("Hasło musi zawierać co najmniej jeden znak niealfanumeryczny.");
        }
    }
}
