using System.Security.Cryptography;
using Support.Auth.Id.Domain.Entity;

namespace Support.Auth.Id.Models.Entity;

public class RefreshToken : BaseModels
{
    public string Token { get; private set; } = string.Empty;
    public DateTime Expires { get; private set; }
    public DateTime Created { get; private set; }
    public string CreatedByIp { get; private set; } = string.Empty;
    public DateTime? Revoked { get; private set; }
    public string RevokedByIp { get; private set; } = string.Empty;
    public string ReplacedByToken { get; private set; } = string.Empty;

    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => Revoked == null && !IsExpired;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private RefreshToken() { }

    public static RefreshToken Create(int  expirationDays, string createdByIp)
    {
        if (expirationDays <= 0) throw new ArgumentException("Expiration days must be greater than zero.");

        if (String.IsNullOrEmpty(createdByIp)) throw new ArgumentException("Ip Address cant be null or empty");

        byte[] randomBytes = new byte[64];

        using RandomNumberGenerator rgn = RandomNumberGenerator.Create();
        rgn.GetBytes(randomBytes);

        string token = Convert.ToBase64String(randomBytes);

        return new RefreshToken
        {
            Token = token,
            Expires = DateTime.UtcNow.AddDays(expirationDays),
            Created = DateTime.UtcNow,
            CreatedByIp = createdByIp
        };
    }

    public void Revoke(string revokedByIp, string? replacedByToken = null)
    {
        if (!IsActive) throw new InvalidOperationException("Refresh token already inactive.");

        Revoked = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken ?? string.Empty;
        SetUpdate();
    }
}
