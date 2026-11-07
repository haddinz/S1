using Support.Auth.Client.DTOs.Response;
using Support.Auth.Id.Common.CommandQuery;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Roles;

public class GetRoleByIdQuery : ICommandQuery<RoleResponse>
{
    public Guid RoleId { get; init; }
}

public class GetRoleByIdQueryHandler : ICommandQueryHandler<GetRoleByIdQuery, RoleResponse>
{
    private readonly ILogger<GetRoleByIdQueryHandler> _logger;
    private readonly IRoleRepositories _roleRepo;

    public GetRoleByIdQueryHandler(
        ILogger<GetRoleByIdQueryHandler> logger,
        IRoleRepositories roleRepo
    )
    {
        _logger = logger;
        _roleRepo = roleRepo;
    }

    public async Task<RoleResponse> HandleAsync(
        GetRoleByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit GetRoleByIdQueryHandler at AuthServices");

        Role role =
            await _roleRepo.GetRoleByIdAsync(query.RoleId, cancellationToken)
            ?? throw new NotFoundException($"Role with id '{query.RoleId}' was not found.");

        RoleResponse roleResponse = new()
        {
            Id = role.Id,
            RoleType = role.RoleType,
            RoleName = role.RoleName,
            Description = role.Description,
        };

        return roleResponse;
    }
}
