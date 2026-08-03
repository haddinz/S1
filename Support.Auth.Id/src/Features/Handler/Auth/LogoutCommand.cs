using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Auth;

public class LogoutCommand : ICommand
{
    public string RefreshTokenValue { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
}

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly ILogger<LogoutCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;

    public LogoutCommandHandler(ILogger<LogoutCommandHandler> logger, IAuthRepositories authRepo)
    {
        _logger = logger;
        _authRepo = authRepo;
    }

    public async Task HandleAsync(LogoutCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit LogoutCommandHandler");

        RefreshToken? refreshToken = await _authRepo.GetRefreshTokenAsync(
            command.RefreshTokenValue,
            cancellationToken
        );

        if (refreshToken is null)
            return;

        if (!refreshToken.IsActive)
            return;

        refreshToken.Revoke(command.IpAddress);
        await _authRepo.SaveChangesAsync(cancellationToken);
    }
}