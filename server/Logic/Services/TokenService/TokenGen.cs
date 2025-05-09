using beautysalon.Database.Models;
using beautysalon.Database;
using beautysalon.Logic.DTOs.Tokens;
using beautysalon.Logic.Services.TokenProvider;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenGen : ITokenGen
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly DatabaseContext _databaseContext;
    private readonly UserManager<Staff> _userManager;

    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(60);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    public TokenGen(IConfiguration config, DatabaseContext databaseContext, UserManager<Staff> userManager)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:key"]));
        _databaseContext = databaseContext;
        _userManager = userManager;
    }

    public async Task<Tokens> CreateTokenAsync(Staff user)
    {
        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

        var claims = new List<Claim>
        {
            new Claim("email", user.Email),
            new Claim("name", user.FirstName),
            new Claim("sid", user.Id.ToString()),
            new Claim("role", role ?? ""),
        };

        var accessToken = CreateJwtToken(claims, AccessTokenLifetime, "access");

        var refreshClaims = new List<Claim>(claims)
        {
            new Claim("token_type", "refresh")
        };

        var refreshToken = CreateJwtToken(refreshClaims, RefreshTokenLifetime, "refresh");

        await StoreOrUpdateRefreshTokenAsync(user.Id, refreshToken);

        return new Tokens
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<Tokens> CreateAccessTokenFromRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var handler = new JsonWebTokenHandler();
            var validationParams = GetTokenValidationParameters();

            var result = await handler.ValidateTokenAsync(refreshToken, validationParams);

            if (!result.IsValid)
                throw new SecurityTokenException("Invalid refresh token.");

            var claims = result.Claims;

            if (claims == null)
                throw new SecurityTokenException("No claims found in token.");

            var userId = claims["sid"];

            var tokenType = claims["token_type"];

            if (string.IsNullOrWhiteSpace(tokenType.ToString()) || tokenType.ToString().Trim() != "refresh")
            {
                throw new SecurityTokenException("Invalid token type.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new SecurityTokenException("User not found.");

            var storedToken = await _databaseContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == user.Id);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                if (storedToken != null)
                {
                    _databaseContext.RefreshTokens.Remove(storedToken);
                    await _databaseContext.SaveChangesAsync();
                }

                throw new SecurityTokenException("Refresh token has expired or is invalid.");
            }

            return await CreateTokenAsync(user);
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

    public async Task<Staff> DecodeToken(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new SecurityTokenException("Token is missing.");

            token = token.StartsWith("Bearer ") ? token["Bearer ".Length..] : token;

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                throw new SecurityTokenException("Malformed token.");

            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sid")?.Value
                ?? throw new SecurityTokenException("Token does not contain user ID.");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new SecurityTokenException($"User with ID {userId} not found.");

            return user;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Token decoding failed: " + ex.Message, ex);
        }
    }

    private string CreateJwtToken(IEnumerable<Claim> claims, TimeSpan lifetime, string tokenType)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(lifetime),
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature),
            Issuer = _config["jwt:issuer"],
            Audience = _config["jwt:audience"]
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private TokenValidationParameters GetTokenValidationParameters() =>
        new()
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

    private async Task StoreOrUpdateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var existing = await _databaseContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

        if (existing != null)
        {
            existing.Token = refreshToken;
            existing.ExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime);
            _databaseContext.RefreshTokens.Update(existing);
        }
        else
        {
            await _databaseContext.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime)
            });
        }

        await _databaseContext.SaveChangesAsync();
    }
}
