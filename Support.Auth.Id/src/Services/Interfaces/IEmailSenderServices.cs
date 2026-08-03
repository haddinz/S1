namespace Support.Auth.Id.Services.Interfaces;

public interface IEmailSenderServices
{
    Task SendEmailVerificationAsync(string email, string fullName, string token);
}
