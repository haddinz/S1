using System;
using Support.Auth.Id.Domain.ValueObject;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.Entity;

namespace Support.Auth.Id.Domain.Entity;

public class User : BaseModels
{
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }

    // Status and Proteksi
    public bool IsActive { get; private set; } = true;
    public int AccessFailedCount { get; private set; } = 0;
    public DateTime? LockoutEnd { get; private set; }

    // Email verivy
    public EmailVerification? EmailVerification { get; private set; }
    public bool IsEmailVerified { get; private set; } = false;

    // Security sesi and autid
    public Guid SecurityStamp { get; private set; } = Guid.NewGuid();
    public DateTime? LastLoginAt { get; private set; }

    public ICollection<Role> Roles { get; private set; } = new List<Role>();
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    private User()
        : base() { }

    public static User Register(
        string email,
        string fullName,
        string userName,
        string passwordHash,
        string? phoneNumber = ""
    )
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BusinessValidationException("Email cant be null.");
        if (string.IsNullOrWhiteSpace(fullName))
            throw new BusinessValidationException("Full Name cant be null.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new BusinessValidationException("Password cant be null.");
        if (string.IsNullOrWhiteSpace(userName))
            throw new BusinessValidationException("UserName cant be null.");

        return new User
        {
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            UserName = userName.Trim(),
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            SecurityStamp = Guid.NewGuid(),
            IsActive = true,
        };
    }

    public void UpdateProfile(string fullName, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new BusinessValidationException("Full name cannot be empty.");
        }

        FullName = fullName.Trim();
        PhoneNumber = phoneNumber?.Trim();

        SetUpdate();
    }

    // Assigne role to user
    public void AssignRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        bool alreadyAssigne = Roles.Any(r => r.Id == role.Id);
        if (alreadyAssigne)
            return;

        Roles.Add(role);
        SetUpdate();
    }

    public void RemoveRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        Role? existingRole = Roles.FirstOrDefault(r => r.Id == role.Id);
        if (existingRole is null)
            return;

        Roles.Remove(existingRole);
        SetUpdate();
    }

    // Logic Bisnis: execution if wrong password
    public void IncrementAccessFailed()
    {
        AccessFailedCount++;
        if (AccessFailedCount >= 5)
        {
            // locked for 15 menutes
            LockoutEnd = DateTime.UtcNow.AddMinutes(15);
        }
        SetUpdate();
    }

    // Logic Bisnis: Reset value while login success
    public void RecordSuccessfulLogin()
    {
        AccessFailedCount = 0;
        LockoutEnd = null;
        LastLoginAt = DateTime.UtcNow;
        SetUpdate();
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        RefreshTokens.Add(refreshToken);
        SetUpdate();
    }

    public RefreshToken GetActiveRefreshToken(string token)
    {
        RefreshToken? refreshToken = RefreshTokens.FirstOrDefault(x => x.Token == token);

        if (refreshToken is null)
        {
            throw new InvalidOperationException("Refresh token not found.");
        }

        if (!refreshToken.IsActive)
        {
            throw new InvalidOperationException("Refresh token inactive.");
        }

        return refreshToken;
    }

    public void ForceLogoutAllDevices(string revokeByIp)
    {
        SecurityStamp = Guid.NewGuid();
        foreach (RefreshToken refreshtoken in RefreshTokens.Where(x => x.IsActive))
        {
            refreshtoken.Revoke(revokeByIp);
        }
        SetUpdate();
    }

    // Validate user active login
    public void ValidateLogin()
    {
        if (!IsActive)
            throw new BusinessValidationException("User In-Active");

        if (LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow)
            throw new BusinessValidationException("User Locked");
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new BusinessValidationException("New Password cant be empty");

        PasswordHash = newPasswordHash;
        SecurityStamp = Guid.NewGuid();
        SetUpdate();
    }

    public void SetEmailVerificationToken(string token, DateTime expiresAt)
    {
        if (IsEmailVerified)
            throw new BusinessValidationException("Email has already been verified.");

        EmailVerification = new EmailVerification(token, expiresAt);

        SetUpdate();
    }

    public void VerivyEmail(string token)
    {
        if (IsEmailVerified)
            throw new InvalidOperationException("Email already verified.");

        if (EmailVerification is null)
            throw new InvalidOperationException("Verification token not found.");

        if (EmailVerification.IsVerified)
            throw new InvalidOperationException("Email already verified.");

        if (EmailVerification.ExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("Verification token expired.");

        if (EmailVerification.Token != token)
            throw new InvalidOperationException("Verification token invalid.");

        EmailVerification?.Verify();
        IsEmailVerified = true;

        SetUpdate();
    }
}
