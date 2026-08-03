using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Handler.Auth;

public class ChangesPasswordCommand : ICommand
{
    public Guid UserId { get; init; }
    public string Password { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
}

public class ChangesPasswordCommandHandler : ICommandHandler<ChangesPasswordCommand>
{
    private readonly ILogger<ChangesPasswordCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IPasswordPolicyValidator _validator;

    public ChangesPasswordCommandHandler(ILogger<ChangesPasswordCommandHandler> logger, IAuthRepositories authRepo, IPasswordPolicyValidator validator)
    {
        _logger = logger;
        _authRepo = authRepo;
        _validator = validator;
    }

    public async Task HandleAsync(ChangesPasswordCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit ChangesPasswordCommandHandler");

        User? user =
            await _authRepo.GetUserByIdAsync(command.UserId)
            ?? throw new NotFoundException("User Not Found");

        user.ValidateLogin();

        bool isValidPassword = BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash);
        if (!isValidPassword)
            throw new UnauthorizedException("Current password Invalid");

        _validator.Validate(command.NewPassword);

        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        user.ChangePassword(newPasswordHash);
        user.ForceLogoutAllDevices(command.IpAddress);

        await _authRepo.SaveChangesAsync(cancellationToken);
    }
}