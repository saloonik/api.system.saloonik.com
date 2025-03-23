using beautysalon.Contracts;
using beautysalon.Database.Models;
using beautysalon.Database;
using beautysalon.Logic.DTOs.ServerResponse;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using beautysalon.Logic.Services.Validators.CompanyValidator;

public class AuthService : IAuthService
{
    private readonly UserManager<Staff> _userManager;
    private readonly ILogger<AuthService> _logger;
    private readonly DatabaseContext _databaseContext;
    private readonly IValidateCompany _validateCompany;
    public AuthService (UserManager<Staff> userManager, ILogger<AuthService> logger, DatabaseContext databaseContext, IValidateCompany validateCompany)
    {
        _userManager = userManager;
        _logger = logger;
        _databaseContext = databaseContext;
        _validateCompany = validateCompany;
    }

    public async Task<ServerResponse> RegisterAsync (RegisterRequest authRequest)
    {
        using var transaction = await _databaseContext.Database.BeginTransactionAsync();
        try
        {
            // Validate Company
            var result = await _validateCompany.ValidateCompanyNIP(authRequest.CompanyNIP);
            if (!result)
            {
                _logger.LogWarning($"The NIP number {authRequest.CompanyNIP} is invalid.");
                return ServerResponse.CreateErrorResponse("NIP jest niepoprawny", StatusCodes.Status400BadRequest);
            }

            // Check if the company already exists
            var checkIfCompanyExist = await _databaseContext.Companies.AnyAsync(x => x.Nip == authRequest.CompanyNIP);
            if (checkIfCompanyExist)
            {
                _logger.LogWarning($"Company with NIP {authRequest.CompanyNIP} already exists.");
                return ServerResponse.CreateErrorResponse("Firma z tym NIPem już istnieje", StatusCodes.Status409Conflict);
            }

            // Create new company
            var company = new Company { Name = authRequest.CompanyName, Nip = authRequest.CompanyNIP };
            await _databaseContext.Companies.AddAsync(company);

            // Check if the user already exists
            var checkIfUserExist = await _userManager.FindByEmailAsync(authRequest.Email);
            if (checkIfUserExist != null)
            {
                _logger.LogWarning($"User with email {authRequest.Email} already exists.");
                await transaction.RollbackAsync();
                return ServerResponse.CreateErrorResponse("Email jest zajęty", StatusCodes.Status409Conflict);
            }

            // Create the new user
            var user = new Staff { Email = authRequest.Email, CompanyId = company.CompanyId, UserName = authRequest.Email};
            var createUserResult = await _userManager.CreateAsync(user, authRequest.Password);
            if (!createUserResult.Succeeded)
            {
                // Log each individual error
                foreach (var error in createUserResult.Errors)
                {
                    _logger.LogWarning($"Error creating user with email {authRequest.Email}: {error.Description}");
                }

                // Optionally, you can also collect the errors to send them in the response
                var errorMessages = createUserResult.Errors.Select(e => e.Description).ToList();

                
                await transaction.RollbackAsync();
                return ServerResponse.CreateErrorResponse("Nie udało się założyć konta", StatusCodes.Status400BadRequest, string.Join(", ", errorMessages));
            }

            // Assign role to the user
            var addRoleResult = await _userManager.AddToRoleAsync(user, "Owner");
            if (!addRoleResult.Succeeded)
            {
                _logger.LogWarning($"Failed to assign role to user with email {authRequest.Email}.");
                await transaction.RollbackAsync();
                return ServerResponse.CreateErrorResponse("Nie udało się przypisać roli", StatusCodes.Status400BadRequest);
            }

            // Commit the transaction
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

   
}
