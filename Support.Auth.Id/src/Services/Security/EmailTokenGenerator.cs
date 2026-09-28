using System.Security.Cryptography;
using Support.Auth.Id.Models.DTOs;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Security;

public class EmailTokenGenerator<T> : ISecureTokenGenerator<T> where T : class
{
    public EmailTokenGenerator() { }

    public SecureTokenGenerator<T> Generate(TimeSpan expiryDuration)
    {
        Span<byte> bytes = stackalloc byte[32];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
        
        string tokenValue = Convert.ToHexString(bytes).ToLowerInvariant();
        DateTime expiryTime = DateTime.UtcNow.Add(expiryDuration);

        return new SecureTokenGenerator<T>(tokenValue, expiryTime);
    }
}
