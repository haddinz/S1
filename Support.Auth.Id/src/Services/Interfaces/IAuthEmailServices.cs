namespace Support.Auth.Id.Services.Interfaces;

public interface IAuthEmailServices
{
    Task SendEmailVerificationAsync(string email, string fullName, string HTMLBody, string token);
    Task SendForgotPasswordAsync(string email, string fullName, string HTMLBody, string token);
}
