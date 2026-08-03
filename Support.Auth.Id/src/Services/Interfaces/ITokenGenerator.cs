using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Domain.Entity;

namespace Support.Auth.Id.Services.Interfaces;

public interface ITokenGenerator
{
    string GeneratorAccessToken(User user);
    RefreshToken GeneratorRefreshToken(string ipAddress);
}
