using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Handler.Auth;

public class ResetPasswordCommand : ICommand
{
    public string Token { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
}

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly ILogger<ResetPasswordCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IPasswordPolicyValidator _validator;
    private readonly IAppHasher _appHasher;

    public ResetPasswordCommandHandler(
        ILogger<ResetPasswordCommandHandler> logger,
        IAuthRepositories authRepo,
        IPasswordPolicyValidator validator,
        IAppHasher appHasher
    )
    {
        _logger = logger;
        _authRepo = authRepo;
        _validator = validator;
        _appHasher = appHasher;
    }

    public async Task HandleAsync(
        ResetPasswordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit ResetPasswordCommandHandler at Support Auth Handler");

        if (string.IsNullOrEmpty(command.Token))
            throw new BusinessValidationException("Password reset token is required.");

        if (string.IsNullOrWhiteSpace(command.NewPassword))
            throw new BusinessValidationException("New password is required.");

        string tokenHash = _appHasher.HashToken(command.Token);

        User? user =
            await _authRepo.GetUserByPasswordResetTokenAsync(tokenHash, cancellationToken)
            ?? throw new NotFoundException("User Not Found");

        if (!user.IsActive)
            throw new BusinessValidationException("User account is inactive.");

        if (user.PasswordReset!.IsExpired)
            throw new BusinessValidationException("Password reset token is expired.");

        _validator.Validate(command.NewPassword);
        string newPasswordHash = _appHasher.Hash(command.NewPassword);

        user.UsePasswordResetToken(tokenHash);
        user.ChangePassword(newPasswordHash);
        user.RecordSuccessfulLogin();

        user.ForceLogoutAllDevices(command.IpAddress);

        await _authRepo.SaveChangesAsync(cancellationToken);
    }
}
