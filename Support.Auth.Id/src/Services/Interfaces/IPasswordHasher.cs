namespace Support.Auth.Id.Services.Interfaces;

public interface IAppHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
    string HashToken(string token);
}
