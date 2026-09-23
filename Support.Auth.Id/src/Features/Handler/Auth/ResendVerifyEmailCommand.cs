using Microsoft.Extensions.Options;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Domain.ValueObject;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Features.Helper;
using Support.Auth.Id.Models.DTOs;
using Support.Auth.Id.Models.Enum;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Handler.Auth;

public class ResendVerifyEmailCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
}

public class ResendVerifyEmailCommandHandler : ICommandHandler<ResendVerifyEmailCommand>
{
    private readonly ILogger<ResendVerifyEmailCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IAuthEmailServices _emailServices;
    private readonly ISecureTokenGenerator<EmailVerificationToken> _emailTokenGenerator;
    private readonly FrontendSettings _frontendSettings;

    public ResendVerifyEmailCommandHandler(
        ILogger<ResendVerifyEmailCommandHandler> logger,
        IAuthRepositories authRepo,
        IAuthEmailServices emailServices,
        ISecureTokenGenerator<EmailVerificationToken> emailTokenGenerator,
        IOptions<FrontendSettings> frontendSettings
    )
    {
        _logger = logger;
        _authRepo = authRepo;
        _emailServices = emailServices;
        _emailTokenGenerator = emailTokenGenerator;
        _frontendSettings = frontendSettings.Value;
    }

    public async Task HandleAsync(
        ResendVerifyEmailCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit ResendVerifyEmailCommandHandler at Handler Support Auth");

        User? user = await _authRepo.GetUserByEmailAsync(command.Email, cancellationToken)
            ?? throw new BusinessValidationException("Email is invalid or not registered.");

        if (user.IsEmailVerified)
            throw new ConflictException("User with this email already verified.");

        SecureTokenGenerator<EmailVerificationToken> emailVerificationToken = _emailTokenGenerator.Generate(TimeSpan.FromMinutes(30));
        user.SetEmailVerificationToken(
            emailVerificationToken.Token,
            emailVerificationToken.ExpiresAt
        );

        await _authRepo.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("--> Verivication base url at {BaseUrl}", _frontendSettings.BaseUrl);
        string verificationUrl =
            $"{_frontendSettings.BaseUrl.TrimEnd('/')}/verify-email/resend?token={Uri.EscapeDataString(emailVerificationToken.Token)}";

        string verifyTemplate = $"{TemplateEnum.VerifyEmail}.html"; 
        string htmlBody = HtmlTemplateEngine.Render(verifyTemplate, new()
        {
            { "FULL_NAME", user.FullName },
            { "VERIFICATION_URL", verificationUrl }
        });

        // dont backward, email just sending if save data to db success
        await _emailServices.SendEmailVerificationAsync(
            user.Email,
            user.FullName,
            htmlBody,
            emailVerificationToken.Token
        );
    }
}
