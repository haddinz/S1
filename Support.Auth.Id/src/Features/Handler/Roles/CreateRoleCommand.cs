using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Roles;

public class CreateRoleCommand : ICommand
{
    public string RoleType { get; init ;} = string.Empty;
    public string RoleName { get; init ;} = string.Empty;
    public string Description { get; init ;} = string.Empty;
}

public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand>
{
    private readonly ILogger _logger;
    private readonly IRoleRepositories _roleRepo;

    public CreateRoleCommandHandler(ILogger logger, IRoleRepositories roleRepo)
    {
        _logger = logger;
        _roleRepo = roleRepo;
    }

    public async Task HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit CreateRoleCommandHandler at AuthServices");

        if(await _roleRepo.IsRoleTypeExists(command.RoleType, cancellationToken))
            throw new ConflictException("Email already exists");

        Role role = Role.Create(command.RoleType, command.RoleName, command.Description);
        await _roleRepo.AddRoleAsync(role, cancellationToken);
    }
}