namespace Support.Auth.Id.Domain.ValueObject;

public sealed class EmailVerification
{
    public string Token { get; private set; } = string.Empty;

    public DateTime? ExpiresAt { get; private set; }

    public DateTime? VerifiedAt { get; private set; }

    public bool IsVerified => VerifiedAt.HasValue;

    private EmailVerification() { }

    internal EmailVerification(string token, DateTime expiresAt)
    {
        Token = token;
        ExpiresAt = expiresAt;
    }

    internal void Verify()
    {
        ExpiresAt = null;
        VerifiedAt = DateTime.UtcNow;
        Token = string.Empty;
    }
}
