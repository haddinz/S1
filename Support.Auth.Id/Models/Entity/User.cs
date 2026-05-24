using System;

namespace Support.Auth.Id.Models.Entity;

public class User : BaseModels
{
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }

    // Status and Proteksi
    public bool IsActive { get; private set; } = true;
    public bool IsEmailVerified { get; private set; } = false;
    public int AccessFailedCount { get; private set; } = 0;
    public DateTime? LockoutEnd { get; private set; }

    // Security sesi and autid
    public Guid SecurityStamp { get; private set; } = Guid.NewGuid();
    public DateTime? LastLoginAt { get; private set; }

    public ICollection<Role> Roles { get; private set; } = new List<Role>();
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    private User()
        : base() { }

    public static User Register(string email, string fullName, string userName, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full Name tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password tidak boleh kosong.");

        return new User
        {
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            UserName = userName.Trim(),
            FullName = fullName.Trim(),
            SecurityStamp = Guid.NewGuid(),
            IsActive = true,
        };
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

    public void ForceLogoutAllDevices()
    {
        SecurityStamp = Guid.NewGuid();
        SetUpdate();
    }

    // Validate user active login
    public void ValidateLogin()
    {
        if (!IsActive)
            throw new InvalidOperationException("User In-Active");

        if (LockoutEnd.HasValue && LockoutEnd.Value > DateTime.Now)
            throw new InvalidOperationException("User Locked");
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password cant be empty");

        PasswordHash = newPasswordHash;
        SecurityStamp = Guid.NewGuid();
        SetUpdate();
    }
}
