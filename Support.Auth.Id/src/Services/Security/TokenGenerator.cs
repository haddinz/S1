using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql.Internal;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Models.Enum;
using Support.Auth.Id.Services.Interfaces;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Domain.ValueObject;

namespace Support.Auth.Id.Features.Security;

public class TokenGenerator : ITokenGenerator
{
    private readonly ILogger<TokenGenerator> _logger;
    private readonly JwtSettings _jwtSettings;

    public TokenGenerator(ILogger<TokenGenerator> logger, IOptions<JwtSettings> jwtSettings)
    {
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    public string GeneratorAccessToken(User user)
    {
        _logger.LogInformation("--> Hit GeneratorAccessToken for user {user}", user.FullName);

        List<Claim> claims = new()
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (Role role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
        }

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        JwtSecurityToken token = new(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GeneratorRefreshToken(string ipAddress)
    {
        _logger.LogInformation("--> Hit Create refresh token for user");

        return RefreshToken.Create(_jwtSettings.RefreshTokenExpirationDays, ipAddress);
    }
}
