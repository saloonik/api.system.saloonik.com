using beautysalon.Database.Models;
using beautysalon.Logic.DTOs.Tokens;

namespace beautysalon.Logic.Services.TokenProvider
{
    public interface ITokenGen

    {
        public Tokens CreateToken(Staff user);
        Task<Tokens> CreateAccessTokenFromRefreshTokenAsync(string refreshToken);
    }
}