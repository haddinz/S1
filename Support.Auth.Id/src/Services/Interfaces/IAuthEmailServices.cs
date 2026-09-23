using Support.Auth.Id.Models;

namespace Support.Auth.Id.Services.Interfaces;

public interface IAuthEmailServices
{
    Task SendEmailVerificationAsync(string email, string fullName, string HTMLBody, string token, CancellationToken cancellationToken = default);
    Task SendEmailAsync(AuthSendEmail authSendEmail, CancellationToken cancellationToken = default);
}
