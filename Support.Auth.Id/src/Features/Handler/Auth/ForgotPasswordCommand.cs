using Hangfire;
using Microsoft.Extensions.Options;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Constans;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Domain.ValueObject;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models;
using Support.Auth.Id.Models.DTOs;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Handler.Auth;

public sealed class ForgotPasswordCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
}

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IAuthEmailServices _emailSender;
    private readonly ISecureTokenGenerator<ForgotPasswordToken> _tokenGenerator;
    private readonly IAppHasher _appHasher;
    private readonly FrontendSettings _options;

    public ForgotPasswordCommandHandler(
        ILogger<ForgotPasswordCommandHandler> logger,
        IAuthRepositories authRepo,
        IAuthEmailServices emailSender,
        IOptions<FrontendSettings> options,
        ISecureTokenGenerator<ForgotPasswordToken> tokenGenerator,
        IAppHasher appHasher
    )
    {
        _logger = logger;
        _authRepo = authRepo;
        _emailSender = emailSender;
        _options = options.Value;
        _tokenGenerator = tokenGenerator;
        _appHasher = appHasher;
    }

    public async Task HandleAsync
    (
        ForgotPasswordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit ForgotPasswordCommandHandler at Support Auth");

        string email = command.Email.Trim().ToLowerInvariant();

        User? user = await _authRepo.GetUserByEmailAsync(email, cancellationToken);

        // Security access for do not reveal whether the account exists
        if (user is null)
        {
            _logger.LogInformation("Password reset email sent if account exists");
            return;
        }

        if (!user.IsActive)
        {
            _logger.LogInformation("user with this email is not verification yet");
            return;
        }

        await using var transaction = await _authRepo.BeginTransactionAsync(cancellationToken);
        try
        {
            SecureTokenGenerator<ForgotPasswordToken> passwordResetToken = _tokenGenerator.Generate(TimeSpan.FromMinutes(30));
            string forgotPassTemplate = Template.Objects.ForgotPassword ??
                throw new NotFoundException("Template email is not found");

            string verificationUrl =
                $"{_options.BaseUrl.TrimEnd('/')}/forgot-password?token={Uri.EscapeDataString(passwordResetToken.Token)}";
            string tokenHash = _appHasher.HashToken(passwordResetToken.Token);

            user.SetPasswordResetToken(tokenHash, passwordResetToken.ExpiresAt);
            await _authRepo.SaveChangesAsync(cancellationToken);

            BackgroundJob.Enqueue<IEmailBackgroundServices>(
                service => service.SendBackgroundEmailAsycn(
                    user.Email,
                    user.FullName,
                    "Forgot Password",
                    forgotPassTemplate,
                    verificationUrl,
                    passwordResetToken.Token,
                    cancellationToken
                )
            );

            await _authRepo.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _authRepo.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Failed to queue password reset for {Email}", user.Email);

            throw;
        }
    }
}
