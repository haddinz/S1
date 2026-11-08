using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Roles;

public class AssignRoleCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid RoleId { get; init; }
}

public class AssignRoleCommandHandler : ICommandHandler<AssignRoleCommand>
{
    private readonly ILogger<AssignRoleCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IRoleRepositories _roleRepo;

    public AssignRoleCommandHandler(
        ILogger<AssignRoleCommandHandler> logger,
        IAuthRepositories authRepo,
        IRoleRepositories roleRepo)
    {
        _logger = logger;
        _authRepo = authRepo;
        _roleRepo = roleRepo;
    }

    public async Task HandleAsync(AssignRoleCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit AssignRoleCommandHandler at AuthServices");

        User user = await _authRepo.GetUserByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException($"User with id '{command.UserId}' was not found.");

        Role role = await _roleRepo.GetTrackedRoleByIdAsync(command.RoleId, cancellationToken)
            ?? throw new NotFoundException($"Role with id '{command.RoleId}' was not found.");

        if (user.Roles.Any(x => x.Id == role.Id))
            throw new ConflictException($"User already has role '{role.RoleName}'.");

        user.AssignRole(role);

        await _authRepo.SaveChangesAsync(cancellationToken);
    }
}
