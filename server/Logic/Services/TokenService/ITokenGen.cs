using beautysalon.Database.Models;

namespace beautysalon.Logic.Services.TokenProvider
{
    public interface ITokenGen

    {
        public string CreateToken(Staff user);

    }
}