using beautysalon.Database.Models;
using beautysalon.Database;
using beautysalon.Logic.DTOs.Tokens;
using beautysalon.Logic.Services.TokenProvider;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

public class TokenGen : ITokenGen
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly DatabaseContext _databaseContext;
    private readonly UserManager<Staff> _userManager;

    public TokenGen(IConfiguration config, DatabaseContext databaseContext, UserManager<Staff> userManager)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:key"]));
        _databaseContext = databaseContext;
        _userManager = userManager;
    }

    public Tokens CreateToken(Staff user)
    {
        var refreshToken = GenerateRefreshTokenJWT(user);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.FirstName),
        new Claim(ClaimTypes.Sid, user.Id.ToString())
    };

        var signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = signingCredentials,
            Issuer = _config["jwt:issuer"],
            Audience = _config["jwt:audience"]
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);

        var existingRefreshToken = _databaseContext.RefreshTokens
            .FirstOrDefault(rt => rt.UserId == user.Id);

        if (existingRefreshToken != null)
        {
            existingRefreshToken.Token = refreshToken;
            existingRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
            _databaseContext.RefreshTokens.Update(existingRefreshToken);
        }
        else
        {
            _databaseContext.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
        }

        _databaseContext.SaveChanges();

        return new Tokens
        {
            AccessToken = handler.WriteToken(token),
            RefreshToken = refreshToken
        };
    }


    private string GenerateRefreshTokenJWT(Staff user)
    {
        var refreshTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["jwt:key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Sid, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("token_type", "refresh")
        }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _config["jwt:issuer"],
            Audience = _config["jwt:audience"]
        };

        var token = refreshTokenHandler.CreateToken(tokenDescriptor);
        return refreshTokenHandler.WriteToken(token);
    }

    public async Task<Tokens> CreateAccessTokenFromRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var tokenHandler = new JsonWebTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _config["jwt:issuer"],
                ValidateAudience = true,
                ValidAudience = _config["jwt:audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var result = tokenHandler.ValidateTokenAsync(refreshToken, tokenValidationParameters);

            var userId = result.Result.Claims.FirstOrDefault(c => c.Key == ClaimTypes.Sid).Value.ToString();
            if (string.IsNullOrEmpty(userId))
                throw new SecurityTokenException("User ID not found in the token.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new SecurityTokenException("User not found.");

            var tokenType = result.Result.Claims.FirstOrDefault(c => c.Key == "token_type").Value.ToString();
            if (tokenType != "refresh")
                throw new SecurityTokenException("Invalid token type.");

            var storedRefreshToken = await _databaseContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == user.Id);

            if (storedRefreshToken == null)
                throw new SecurityTokenException("Invalid refresh token.");

            if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                _databaseContext.RefreshTokens.Remove(storedRefreshToken);
                await _databaseContext.SaveChangesAsync();
                throw new SecurityTokenException("Refresh token has expired.");
            }

            return CreateToken(user);
        }
        catch (SecurityTokenException ex)
        {
            throw new UnauthorizedAccessException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An unexpected error occurred during token renewal.", ex);
        }
    }
}
