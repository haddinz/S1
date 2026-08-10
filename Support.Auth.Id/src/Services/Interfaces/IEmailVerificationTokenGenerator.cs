using Support.Auth.Id.Models.DTOs;

namespace Support.Auth.Id.Services.Interfaces;

public interface IEmailVerificationTokenGenerator
{
    EmailVerificationToken Generate();
}
