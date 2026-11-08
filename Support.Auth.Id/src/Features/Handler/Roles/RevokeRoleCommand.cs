using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Roles;

public class RevokeRoleCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid RoleId { get; init; }
}

public class RevokeRoleCommandHandler : ICommandHandler<RevokeRoleCommand>
{
    private readonly ILogger<RevokeRoleCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IRoleRepositories _roleRepo;

    public RevokeRoleCommandHandler(
        ILogger<RevokeRoleCommandHandler> logger,
        IAuthRepositories authRepo,
        IRoleRepositories roleRepo)
    {
        _logger = logger;
        _authRepo = authRepo;
        _roleRepo = roleRepo;
    }

    public async Task HandleAsync(RevokeRoleCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit RevokeRoleCommandHandler at AuthServices");

        User user = await _authRepo.GetUserByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException($"User with id '{command.UserId}' was not found.");

        Role role = await _roleRepo.GetTrackedRoleByIdAsync(command.RoleId, cancellationToken)
            ?? throw new NotFoundException($"Role with id '{command.RoleId}' was not found.");

        if (!user.Roles.Any(x => x.Id == role.Id))
            throw new BusinessValidationException($"User does not have the role '{role.RoleName}'.");

        user.RemoveRole(role);

        await _authRepo.SaveChangesAsync(cancellationToken);
    }
}
