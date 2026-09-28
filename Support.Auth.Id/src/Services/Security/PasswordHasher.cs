using System.Security.Cryptography;
using System.Text;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Security;

public class AppHasher : IAppHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public string HashToken(string token)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(token);
        byte[] hashBytes = SHA256.HashData(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
