using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Security;

public sealed class PasswordPolicyValidator : IPasswordPolicyValidator
{
    public void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new BusinessValidationException("Password must be at least 8 characters.");
        }

        bool 
            hasUpper = false,
            hasLower = false,
            hasDigit = false,
            hasSpecial = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c))
                hasUpper = true;
            else if (char.IsLower(c))
                hasLower = true;
            else if (char.IsDigit(c))
                hasDigit = true;
            else if (char.IsPunctuation(c) || char.IsSymbol(c))
                hasSpecial = true;

            if (hasUpper && hasLower && hasDigit && hasSpecial)
                break;
        }

        if (!hasUpper)
            throw new BusinessValidationException("Password must contain uppercase letter.");
        if (!hasLower)
            throw new BusinessValidationException("Password must contain lowercase letter.");
        if (!hasDigit)
            throw new BusinessValidationException("Password must contain number.");
        if (!hasSpecial)
            throw new BusinessValidationException("Password must contain special character.");
    }
}
