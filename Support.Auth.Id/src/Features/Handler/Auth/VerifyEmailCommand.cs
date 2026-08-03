using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Auth;

public class VerifyEmailCommand : ICommand
{
    public string Token { get; init; } = string.Empty;
}

public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
{
    private readonly ILogger<VerifyEmailCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;

    public VerifyEmailCommandHandler(ILogger<VerifyEmailCommandHandler> logger, IAuthRepositories authRepo)
    {
        _logger = logger;
        _authRepo = authRepo;
    }

    public async Task HandleAsync(VerifyEmailCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit VerifyEmailCommandHandler");

        User? user = await _authRepo.GetUserByEmailVerificationTokenAsync(command.Token) 
            ?? throw new NotFoundException("Verification token is invalid.");

        user.VerivyEmail(command.Token);
        await _authRepo.SaveChangesAsync(cancellationToken);
    }
}