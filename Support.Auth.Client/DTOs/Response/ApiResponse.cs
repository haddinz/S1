namespace Support.Auth.Client.DTOs.Response;

public sealed record ApiResponse<T>(bool Success, string Message, T? Data = default);

public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(T data, string message = "Success") =>
        new(true, message, data);

    public static ApiResponse<object> Success(string message = "Success") =>
        new(true, message, null);

    public static ApiResponse<object> Failure(string message = "Failed") =>
        new(false, message, null);

    public static ApiResponse<Dictionary<string, string[]>> Failure(
        Dictionary<string, string[]> validationErrors,
        string message = "Validation failed"
    )
    {
        return new ApiResponse<Dictionary<string, string[]>>(false, message, validationErrors);
    }
}
