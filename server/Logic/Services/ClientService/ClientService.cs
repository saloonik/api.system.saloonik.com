using beautysalon.Database;
using beautysalon.Database.Models;
using beautysalon.Logic.DTOs;
using beautysalon.Logic.DTOs.ServerResponse;
using beautysalon.Logic.Services.TokenProvider;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace beautysalon.Logic.Services.ClientService
{
    public class ClientService : IClientService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<ClientService> _logger;
        private readonly ITokenGen _tokenGen;
        private readonly IValidator<ClientDTO> _clientValidator;

        public ClientService(
            DatabaseContext databaseContext,
            ILogger<ClientService> logger,
            ITokenGen tokenGen,
            IValidator<ClientDTO> clientValidator)
        {
            _databaseContext = databaseContext;
            _logger = logger;
            _tokenGen = tokenGen;
            _clientValidator = clientValidator;
        }

        public async Task<ServerResponse> AddClientAsync(ClientDTO client, string token)
        {
            try
            {
                // Validate the client using FluentValidation
                var validation = await _clientValidator.ValidateAsync(client);
                if (!validation.IsValid)
                {
                    return ServerResponse.CreateValidationFailedResponse(validation.Errors);
                }

                // Decode the token and get the company details
                var decodedUserToken = await _tokenGen.DecodeToken(token);

                var company = await _databaseContext.Companies
                     .Include(c => c.Staff)
                     .Where(c => c.Staff.Any(s => s.Id == decodedUserToken.Id))
                     .FirstOrDefaultAsync();

                // Check if the client already exists
                var existingClient = await _databaseContext.Clients
                    .FirstOrDefaultAsync(c => c.PhoneNumber == client.PhoneNumber);

                if (existingClient != null)
                {
                    return ServerResponse.CreateErrorResponse("Klient z danym numerem telefonu juz istnieje.", StatusCodes.Status409Conflict);
                }

                // Create and save the new client
                var newClient = new Client
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    PhoneNumber = client.PhoneNumber,
                    Email = client.Email,
                    Company = company
                };

                await _databaseContext.Clients.AddAsync(newClient);
                await _databaseContext.SaveChangesAsync();

                return new ServerResponse
                {
                    IsSuccess = true,
                    ResultTitle = "Dodano klienta",
                    StatusCode = StatusCodes.Status201Created,
                    StatusMessage = "Dodano klienta",
                    ResultDescription = "Klient został dodany pomyślnie"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas dodawania klienta.");
                return ServerResponse.CreateErrorResponse("Wystąpił błąd podczas dodawania klienta", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
