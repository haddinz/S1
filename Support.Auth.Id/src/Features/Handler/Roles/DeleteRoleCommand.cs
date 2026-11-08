using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Roles;

public class DeleteRoleCommand : ICommand
{
    public Guid RoleId { get; init; }
}

public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
{
    private readonly ILogger<DeleteRoleCommandHandler> _logger;
    private readonly IRoleRepositories _roleRepo;

    public DeleteRoleCommandHandler(ILogger<DeleteRoleCommandHandler> logger, IRoleRepositories roleRepo)
    {
        _logger = logger;
        _roleRepo = roleRepo;
    }

    public async Task HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit DeleteRoleCommandHandler at AuthServices");

        Role role = await _roleRepo.GetTrackedRoleByIdAsync(command.RoleId, cancellationToken)
            ?? throw new NotFoundException($"Role with id '{command.RoleId}' was not found.");

        if (await _roleRepo.IsRoleInUseAsync(role.Id, cancellationToken))
            throw new BusinessValidationException("Cannot delete a role that is currently assigned to users.");

        role.SetSoftDelete();

        await _roleRepo.SaveChangesAsync(cancellationToken);
    }
}
