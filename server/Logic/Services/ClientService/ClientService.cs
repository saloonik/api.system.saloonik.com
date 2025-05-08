using beautysalon.Database;
using beautysalon.Database.Models;
using beautysalon.Logic.DTOs;
using beautysalon.Logic.DTOs.ServerResponse;
using beautysalon.Logic.Services.TokenProvider;
using Microsoft.EntityFrameworkCore;

namespace beautysalon.Logic.Services.ClientService
{
    public class ClientService : IClientService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<ClientService> _logger;
        private readonly ITokenGen _tokenGen;

        public ClientService(DatabaseContext databaseContext, ILogger<ClientService> logger, ITokenGen tokenGen)
        {
            _databaseContext = databaseContext;
            _logger = logger;
            _tokenGen = tokenGen;
        }

        public async Task<ServerResponse> AddClientAsync(ClientDTO client, string token)
        {
            try
            {
                var decodedToken = await _tokenGen.DecodeToken(token);

                var company = await _databaseContext.Companies
                    .Include(c => c.Staff)
                    .FirstOrDefaultAsync(c => c.CompanyId == decodedToken.CompanyId);

                var existingClient = await _databaseContext.Clients.FirstOrDefaultAsync(c => c.PhoneNumber == client.PhoneNumber);

                if (existingClient != null)
                {
                    return ServerResponse.CreateErrorResponse("Klient z danym numerem telefonu juz istnieje.", StatusCodes.Status409Conflict);
                }

                var newClient = new Client
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    PhoneNumber = client.PhoneNumber,
                    Email = client.Email,
                    CompanyId = decodedToken.CompanyId,
                    Company = company,
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
