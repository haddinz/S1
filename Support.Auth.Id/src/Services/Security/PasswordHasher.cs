using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Security;

public class AppHasher : IAppHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
