using beautysalon.AuthContracts;
using beautysalon.Contracts;
using beautysalon.Database.Models;
using beautysalon.Logic.DTOs.ServerResponse;

public interface IAuthService
{
    Task<ServerResponse> RegisterAsync (RegisterRequest authRequest);
    Task<ServerResponse> LoginAsync(LoginRequest authRequest);
    Task<ServerResponse> GetAccessTokenFromRefreshTokenAsync(string refreshToken);
}