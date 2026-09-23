using Support.Auth.Id.Common.CommandQuery;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Models.DTOs.Request;
using Support.Auth.Id.Models.DTOs.Response;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Users;

public class GetUsersQuery : ICommandQuery<PagedResponse<UserProfileResponse>>
{
    public PaginationRequest Request { get; set; }

    public GetUsersQuery(PaginationRequest request)
    {
        Request = request;
    }
}

public class GetUsersQueryHandler : ICommandQueryHandler<GetUsersQuery, PagedResponse<UserProfileResponse>>
{
    private readonly ILogger<GetUsersQueryHandler> _logger;
    private readonly IAuthRepositories _authRepo;

    public GetUsersQueryHandler(ILogger<GetUsersQueryHandler> logger, IAuthRepositories authRepo)
    {
        _logger = logger;
        _authRepo = authRepo;
    }
    
    public async Task<PagedResponse<UserProfileResponse>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("--> Hit GetUsersQueryHandler");

        (IReadOnlyList<User> users, int totalRecords) = await _authRepo.GetPagedUsersAsync(
            query.Request.PageNumber,
            query.Request.PageSize,
            cancellationToken
        );

        List<UserProfileResponse> result =
        [
            .. users.Select(x => new UserProfileResponse
            {
                Id = x.Id,
                Email = x.Email,
                UserName = x.UserName,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                IsActive = x.IsActive,
                IsEmailVerified = x.IsEmailVerified,
            }),
        ];

        return PagedResponse<UserProfileResponse>.Create(
            result,
            query.Request.PageNumber,
            query.Request.PageSize,
            totalRecords,
            "Users retrieved successfully."
        );
    }
}
