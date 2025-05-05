using beautysalon.Contracts;
using beautysalon.Database.Models;
using beautysalon.Database;
using beautysalon.Logic.DTOs.ServerResponse;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using beautysalon.Logic.Services.Validators.CompanyValidator;
using beautysalon.AuthContracts;
using beautysalon.Logic.Services.TokenProvider;

public class AuthService : IAuthService
{
    private readonly UserManager<Staff> _userManager;
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<AuthService> _logger;
    private readonly IValidateCompany _validateCompany;
    private readonly ITokenGen _tokenGen;

    public AuthService(UserManager<Staff> userManager, ILogger<AuthService> logger, DatabaseContext databaseContext, IValidateCompany validateCompany, ITokenGen tokenGen)
    {

        _userManager = userManager;
        _logger = logger;
        _databaseContext = databaseContext;
        _validateCompany = validateCompany;
        _tokenGen = tokenGen;
    }
    public async Task<ServerResponse> LoginAsync (LoginRequest authRequest)
    {
        var user = await _userManager.FindByEmailAsync(authRequest.Email);
        if (user is null)
        {
            _logger.LogWarning($"Login failed for email {authRequest.Email}: user not found.");
            return ServerResponse.CreateErrorResponse("Niepoprawny email lub hasło", StatusCodes.Status401Unauthorized);
        }
        var result = await _userManager.CheckPasswordAsync(user, authRequest.Password);

        if (!result)
        {
            _logger.LogWarning($"Login failed for email {authRequest.Email}: incorrect password.");
            return ServerResponse.CreateErrorResponse("Niepoprawny email lub hasło", StatusCodes.Status401Unauthorized);
        }

        var token = _tokenGen.CreateToken(user);

        return new ServerResponse
        {
            IsSuccess = true,
            ResultTitle = "Zalogowano pomyślnie",
            StatusCode = StatusCodes.Status200OK,
            StatusMessage = "Zalogowano pomyślnie",
            ResultDescription = "Zalogowano",
            Token = token,
        };
    }
    public async Task<ServerResponse> RegisterAsync(RegisterRequest authRequest)
    {
        using var transaction = await _databaseContext.Database.BeginTransactionAsync();
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(authRequest.Email);
            if (existingUser != null)
            {
                _logger.LogWarning($"User with email {authRequest.Email} already exists.");
                return ServerResponse.CreateErrorResponse("Email jest zajęty", StatusCodes.Status409Conflict);
            }

            // Validate company NIP
            var isNipValid = await _validateCompany.ValidateCompanyNIP(authRequest.CompanyNIP);
            if (!isNipValid)
            {
                _logger.LogWarning($"The NIP number {authRequest.CompanyNIP} is invalid.");
                return ServerResponse.CreateErrorResponse("NIP jest niepoprawny", StatusCodes.Status400BadRequest);
            }

            // Check if company already exists
            var companyExists = await _databaseContext.Companies.AnyAsync(c => c.Nip == authRequest.CompanyNIP);
            if (companyExists)
            {
                _logger.LogWarning($"Company with NIP {authRequest.CompanyNIP} already exists.");
                return ServerResponse.CreateErrorResponse("Firma z tym NIPem już istnieje", StatusCodes.Status409Conflict);
            }

            // Create and save company
            var company = new Company
            {
                Name = authRequest.CompanyName,
                Nip = authRequest.CompanyNIP,
                Street = authRequest.Street,
                StreetNumber = authRequest.StreetNumber,
                City = authRequest.City,
                PostalCode = authRequest.PostalCode,
                Country = authRequest.Country,

            };
            await _databaseContext.Companies.AddAsync(company);
            await _databaseContext.SaveChangesAsync(); // Save to generate CompanyId

            // Parse full name
            var (firstName, lastName) = ParseFullName(authRequest.Name);

            // Create user
            var user = new Staff
            {
                Email = authRequest.Email,
                UserName = authRequest.Email,
                FirstName = firstName,
                LastName = lastName,
                Company = company
            };

            var createUserResult = await _userManager.CreateAsync(user, authRequest.Password);
            if (!createUserResult.Succeeded)
            {
                foreach (var error in createUserResult.Errors)
                {
                    _logger.LogWarning($"Error creating user with email {authRequest.Email}: {error.Description}");
                }

                var errorMessages = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                await transaction.RollbackAsync();
                return ServerResponse.CreateErrorResponse("Nie udało się założyć konta", StatusCodes.Status400BadRequest, errorMessages);
            }

            // Assign role
            var addRoleResult = await _userManager.AddToRoleAsync(user, "Owner");
            if (!addRoleResult.Succeeded)
            {
                _logger.LogWarning($"Failed to assign role to user {authRequest.Email}.");
                await transaction.RollbackAsync();
                return ServerResponse.CreateErrorResponse("Nie udało się przypisać roli", StatusCodes.Status400BadRequest);
            }

            await transaction.CommitAsync();

            _logger.LogInformation($"User {authRequest.Email} successfully registered with company {authRequest.CompanyName}.");
            return new ServerResponse
            {
                IsSuccess = true,
                ResultTitle = "Pomyślnie założono konto",
                StatusCode = StatusCodes.Status201Created,
                StatusMessage = "Pomyślnie założono firmę oraz konto właściciela",
                ResultDescription = "Założono"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, $"An error occurred while registering user with email {authRequest.Email}.");
            return ServerResponse.CreateErrorResponse("Wystąpił błąd podczas rejestracji", StatusCodes.Status500InternalServerError, ex.Message);
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
