using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Users;

public class UpdateUserCommand : ICommand
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string? PhoneNumber { get; set; }

    public UpdateUserCommand(Guid userId, string fullName, string? phoneNumber)
    {
        UserId = userId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
    }
}

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;

    public UpdateUserCommandHandler(
        ILogger<UpdateUserCommandHandler> logger,
        IAuthRepositories authRepo
    )
    {
        _logger = logger;
        _authRepo = authRepo;
    }

    public async Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit UpdateUserCommandHandler");

        User? user =
            await _authRepo.GetUserByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException($"User With Id {command.UserId} Is Not Found");

        user.UpdateProfile(command.FullName, command.PhoneNumber);

        await _authRepo.SaveChangesAsync();
    }
}
