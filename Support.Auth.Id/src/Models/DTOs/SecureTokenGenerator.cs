namespace Support.Auth.Id.Models.DTOs;

public sealed record EmailVerificationToken;
public sealed record ForgotPasswordToken;

public sealed record SecureTokenGenerator<T>(string Token, DateTime ExpiresAt); 
