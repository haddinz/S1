namespace Support.Auth.Id.Models.DTOs.Request;

public sealed record PaginationRequest
{
    private const int MaxPageSize = 100;
    public int PageNumber { get; init; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
