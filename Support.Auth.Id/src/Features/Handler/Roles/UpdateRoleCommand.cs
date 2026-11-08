using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Roles;

public class UpdateRoleCommand : ICommand
{
    public Guid RoleId { get; init; }
    public string RoleType { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand>
{
    private readonly ILogger<UpdateRoleCommandHandler> _logger;
    private readonly IRoleRepositories _roleRepo;

    public UpdateRoleCommandHandler(ILogger<UpdateRoleCommandHandler> logger, IRoleRepositories roleRepo)
    {
        _logger = logger;
        _roleRepo = roleRepo;
    }

    public async Task HandleAsync(UpdateRoleCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit UpdateRoleCommandHandler at AuthServices");

        Role role = await _roleRepo.GetTrackedRoleByIdAsync(command.RoleId, cancellationToken)
            ?? throw new NotFoundException($"Role with id '{command.RoleId}' was not found.");

        string roleType = command.RoleType.Trim();
        string roleName = command.RoleName.Trim();

        if (!string.Equals(role.RoleType, roleType, StringComparison.Ordinal)
            && await _roleRepo.IsRoleTypeExists(roleType, cancellationToken))
            throw new ConflictException($"Role type '{roleType}' already exists.");

        if (!string.Equals(role.RoleName, roleName, StringComparison.Ordinal)
            && await _roleRepo.IsRoleNameExists(roleName, cancellationToken))
            throw new ConflictException($"Role name '{roleName}' already exists.");

        role.Update(roleType, roleName, command.Description);

        await _roleRepo.SaveChangesAsync(cancellationToken);
    }
}
