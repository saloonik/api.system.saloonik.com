using beautysalon.Database.Models;
using beautysalon.Logic.DTOs.Tokens;

namespace beautysalon.Logic.Services.TokenProvider
{
    public interface ITokenGen

    {
        public Task<Tokens> CreateTokenAsync(Staff user);
        Task<Tokens> CreateAccessTokenFromRefreshTokenAsync(string refreshToken);
        Task<Staff> DecodeToken(string token);
    }
}