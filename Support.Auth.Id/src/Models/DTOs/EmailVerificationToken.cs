namespace Support.Auth.Id.Models.DTOs;

public sealed record EmailVerificationToken(string Token, DateTime ExpiresAt);
