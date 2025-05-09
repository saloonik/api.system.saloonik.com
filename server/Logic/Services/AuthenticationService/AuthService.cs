using beautysalon.Contracts;
using beautysalon.Database.Models;
using beautysalon.Database;
using beautysalon.Logic.DTOs.ServerResponse;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using beautysalon.AuthContracts;
using beautysalon.Logic.Services.TokenProvider;
using beautysalon.Logic.DTOs.Tokens;
using FluentValidation;

public class AuthService : IAuthService
{
    private readonly UserManager<Staff> _userManager;
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<AuthService> _logger;

    private readonly ITokenGen _tokenGen;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthService(UserManager<Staff> userManager, ILogger<AuthService> logger, DatabaseContext databaseContext, ITokenGen tokenGen, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator)
    {
        _userManager = userManager;
        _logger = logger;
        _databaseContext = databaseContext;
        _tokenGen = tokenGen;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }
    public async Task<ServerResponse> LoginAsync(LoginRequest authRequest)
    {
        try
        {
            var validationResult = await _loginValidator.ValidateAsync(authRequest);
            if (!validationResult.IsValid)
                return ServerResponse.CreateValidationFailedResponse(validationResult.Errors);

            var user = await _userManager.FindByEmailAsync(authRequest.Email);
            if (user is null)
            {
                _logger.LogWarning("Login failed: user not found for email {Email}", authRequest.Email);
                return ServerResponse.CreateUnauthorizedResponse("Podany e-mail nie istnieje w systemie.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, authRequest.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Login failed: invalid password for email {Email}", authRequest.Email);
                return ServerResponse.CreateUnauthorizedResponse("Hasło jest niepoprawne.");
            }

            Tokens token = await _tokenGen.CreateTokenAsync(user);

            return new ServerResponse
            {
                IsSuccess = true,
                ResultTitle = "Zalogowano pomyślnie",
                StatusCode = StatusCodes.Status200OK,
                StatusMessage = "Zalogowano",
                ResultDescription = "Dane logowania są poprawne.",
                Token = token.AccessToken,
                RefreshToken = token.RefreshToken
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for {Email}", authRequest.Email);
            return ServerResponse.CreateInternalErrorResponse("Wystąpił błąd podczas logowania.");
        }
    
    }


    public async Task<ServerResponse> RegisterAsync(RegisterRequest authRequest)
    {
        using var transaction = await _databaseContext.Database.BeginTransactionAsync();
        try
        {
            var validationResult = await _registerValidator.ValidateAsync(authRequest);
            if (!validationResult.IsValid)
                return ServerResponse.CreateValidationFailedResponse(validationResult.Errors);

            if (await _userManager.FindByEmailAsync(authRequest.Email) is not null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already in use", authRequest.Email);
                return ServerResponse.CreateConflictResponse("Email jest już w użyciu.");
            }

            if (await _databaseContext.Companies.AnyAsync(c => c.Nip == authRequest.CompanyNIP))
            {
                _logger.LogWarning("Registration failed: Company NIP {NIP} already exists", authRequest.CompanyNIP);
                return ServerResponse.CreateConflictResponse("Firma z podanym NIP już istnieje.");
            }

            var company = new Company
            {
                Name = authRequest.CompanyName,
                Nip = authRequest.CompanyNIP,
                Street = authRequest.Street,
                State = authRequest.State,
                City = authRequest.City,
                PostalCode = authRequest.PostalCode,
                Country = authRequest.Country,
            };

            await _databaseContext.Companies.AddAsync(company);
            await _databaseContext.SaveChangesAsync();

            var (firstName, lastName) = ParseFullName(authRequest.Name);

            var user = new Staff
            {
                Email = authRequest.Email.Trim().ToLowerInvariant(),
                FirstName = firstName,
                UserName = authRequest.Email,
                LastName = lastName,
                Company = company
            };

            var createUserResult = await _userManager.CreateAsync(user, authRequest.Password);

            if (!createUserResult.Succeeded)
            {
                await transaction.RollbackAsync();
                var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                _logger.LogWarning("User creation failed for {Email}: {Errors}", authRequest.Email, errors);
                return ServerResponse.CreateBadRequestResponse($"Błąd tworzenia konta: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Owner");

            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning("Failed to assign Owner role to {Email}", authRequest.Email);
                return ServerResponse.CreateBadRequestResponse("Nie udało się przypisać roli właściciela.");
            }

            await transaction.CommitAsync();

            _logger.LogInformation("User {Email} registered successfully", authRequest.Email);
            return new ServerResponse
            {
                IsSuccess = true,
                ResultTitle = "Rejestracja zakończona sukcesem",
                StatusCode = StatusCodes.Status201Created,
                StatusMessage = "Konto oraz firma zostały pomyślnie utworzone",
                ResultDescription = "Rejestracja zakończona sukcesem"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Unexpected error during registration for {Email}", authRequest.Email);
            return ServerResponse.CreateInternalErrorResponse("Wystąpił błąd podczas rejestracji.");
        }
    }

    public async Task<ServerResponse> RefreshAccessTokenAsync(string refreshToken)
    {
        try
        {
            Tokens token = await _tokenGen.CreateAccessTokenFromRefreshTokenAsync(refreshToken);

            return new ServerResponse
            {
                IsSuccess = true,
                ResultTitle = "Token wygenerowany pomyślnie",
                StatusCode = StatusCodes.Status200OK,
                StatusMessage = "Token wygenerowany pomyślnie",
                ResultDescription = "Wygenerowano token",
                Token = token.AccessToken,
                RefreshToken = token.RefreshToken,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating access token from refresh token");
            return ServerResponse.CreateInternalErrorResponse("Nie udało się odświeżyć tokenu dostępu.");
        }
    }
    

    private (string FirstName, string LastName) ParseFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return ("", "");

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
            return (parts[0], "");

        return (parts[0], string.Join(" ", parts.Skip(1)));
    }

}
