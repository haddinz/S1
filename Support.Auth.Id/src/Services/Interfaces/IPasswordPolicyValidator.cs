namespace Support.Auth.Id.Services.Interfaces;

public interface IPasswordPolicyValidator
{
    void Validate(string password);
}