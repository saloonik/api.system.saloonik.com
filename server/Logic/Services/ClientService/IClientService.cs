using beautysalon.Database.Models;
using beautysalon.Logic.DTOs;
using beautysalon.Logic.DTOs.ServerResponse;

namespace beautysalon.Logic.Services.ClientService
{
    public interface IClientService
    {
        Task<ServerResponse> AddClientAsync(ClientDTO client, string token);
        Task<ServerResponse> DeleteClientByIdAsync(string id, string token);
    }
}