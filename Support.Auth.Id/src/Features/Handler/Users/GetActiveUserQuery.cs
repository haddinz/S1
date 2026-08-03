using Support.Auth.Id.Common.CommandQuery;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.DTOs.Response;
using Support.Auth.Id.Repositories.Interfaces;

namespace Support.Auth.Id.Features.Handler.Users;

public class GetActiveUserQuery : ICommandQuery<UserProfileResponse>
{
    public Guid UserId { get; set; }

    public GetActiveUserQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class GetActiveUserQueryHandler : ICommandQueryHandler<GetActiveUserQuery, UserProfileResponse>
{
    private readonly ILogger<GetActiveUserQueryHandler> _logger;
    private readonly IAuthRepositories _authRepo;

    public GetActiveUserQueryHandler(ILogger<GetActiveUserQueryHandler> logger, IAuthRepositories authRepo)
    {
        _logger = logger;
        _authRepo = authRepo;
    }

    public async Task<UserProfileResponse> HandleAsync(GetActiveUserQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("--> Hit GetActiveUserQueryHandler");

        User? user =
            await _authRepo.GetUserByIdAsync(query.UserId, cancellationToken)
            ?? throw new NotFoundException($"User With IGetCurrentUserQueryd {query.UserId} Is Not Found");

        UserProfileResponse response = new()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            IsEmailVerified = user.IsEmailVerified,
            IsActive = user.IsActive,
        };

        return response;
    }
}