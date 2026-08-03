using Support.Auth.Id.Domain.ValueObject;

namespace Support.Auth.Id.Models.DTOs.Response;

public sealed record PagedResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public IReadOnlyCollection<T> Data { get; init; } = Array.Empty<T>();
    public PaginationMetadata Pagination { get; init; } = default!;

    public static PagedResponse<T> Create(
        IReadOnlyCollection<T> data,
        int pageNumber,
        int pageSize,
        int totalRecords,
        string message = "Success"
    ) 
    {
        int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        return new PagedResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Pagination = new PaginationMetadata
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            }
        };
    }
}
