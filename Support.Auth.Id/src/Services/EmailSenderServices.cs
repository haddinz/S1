using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Services.io;

public class EmailSenderServices : IEmailSenderServices
{
    
    public async Task SendEmailVerificationAsync(string email, string fullName, string token)
    {
        throw new NotImplementedException();
    }
}