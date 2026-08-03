using System.Security.Cryptography;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Security;

public class EmailVerificationTokenGenerator : IEmailVerificationTokenGenerator
{
    public string Generate()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }
}

