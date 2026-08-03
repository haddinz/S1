namespace Support.Auth.Id.Services.Interfaces;

public interface IEmailVerificationTokenGenerator
{
    string Generate();
}