namespace Support.Auth.Id.Services.Security;

public static class PasswordPolicyValidator
{
    public static void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters.");
        }

        bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (char.IsPunctuation(c) || char.IsSymbol(c)) hasSpecial = true;

            if (hasUpper && hasLower && hasDigit && hasSpecial) break;
        }
        
        if (!hasUpper) throw new ArgumentException("Password must contain uppercase letter.");
        if (!hasLower) throw new ArgumentException("Password must contain lowercase letter.");
        if (!hasDigit) throw new ArgumentException("Password must contain number.");
        if (!hasSpecial) throw new ArgumentException("Password must contain special character.");
    }
}
