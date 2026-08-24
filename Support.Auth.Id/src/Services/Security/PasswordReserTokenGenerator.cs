using System.Security.Cryptography;
using Support.Auth.Id.Models.DTOs;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Security;

public class PasswordReserTokenGenerator : ISecureTokenGenerator<PasswordResetToken>
{
    public PasswordResetToken Generate()
    {
        // Previously
        // return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);

        return new PasswordResetToken(Convert.ToHexString(bytes), DateTime.UtcNow.AddHours(1));
    }
}