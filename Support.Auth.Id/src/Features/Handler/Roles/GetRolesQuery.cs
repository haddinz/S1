using Support.Auth.Client.DTOs.Response;
using Support.Auth.Id.Common.CommandQuery;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Roles;

public class GetRolesQuery : ICommandQuery<PagedResponse<RoleResponse>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetRolesQueryHandler
    : ICommandQueryHandler<GetRolesQuery, PagedResponse<RoleResponse>>
{
    private readonly ILogger<GetRolesQueryHandler> _logger;
    private readonly IRoleRepositories _roleRepo;

    public GetRolesQueryHandler(ILogger<GetRolesQueryHandler> logger, IRoleRepositories roleRepo)
    {
        _logger = logger;
        _roleRepo = roleRepo;
    }

    public async Task<PagedResponse<RoleResponse>> HandleAsync(
        GetRolesQuery query,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit GetRolesQueryHandler at AuthServices");

        (IReadOnlyList<Role> roles, int totalRecords) =
            await _roleRepo.GetPagedRolesAsync(
                query.PageNumber,
                query.PageSize,
                cancellationToken
            );

        List<RoleResponse> result =
        [
            .. roles.Select(x => new RoleResponse
            {
                Id = x.Id,
                RoleType = x.RoleType,
                RoleName = x.RoleName,
                Description = x.Description,
            }),
        ];

        return PagedResponse<RoleResponse>.Create(
            result,
            query.PageNumber,
            query.PageSize,
            totalRecords,
            "Roles retrieved successfully."
        );
    }
}
