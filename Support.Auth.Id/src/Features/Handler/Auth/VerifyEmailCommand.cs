using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Handler.Auth;

public class VerifyEmailCommand : ICommand
{
    public string Token { get; init; } = string.Empty;
}

public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
{
    private readonly ILogger<VerifyEmailCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IAppHasher _appHasher;

    public VerifyEmailCommandHandler(ILogger<VerifyEmailCommandHandler> logger, IAuthRepositories authRepo, IAppHasher appHasher)
    {
        _logger = logger;
        _authRepo = authRepo;
        _appHasher = appHasher;
    }

    public async Task HandleAsync(VerifyEmailCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit VerifyEmailCommandHandler");

        string tokenHash = _appHasher.Hash(command.Token);
        User? user = await _authRepo.GetUserByEmailVerificationTokenAsync(tokenHash) 
            ?? throw new NotFoundException("Verification token is invalid.");

        user.VerifyEmail(tokenHash);
        await _authRepo.SaveChangesAsync(cancellationToken);
    }
}