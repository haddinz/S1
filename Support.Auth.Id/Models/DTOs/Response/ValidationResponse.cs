namespace Support.Auth.Id.Models.Dtos.Response;

public class ValidationResponse<T>
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public PagingResponse? PagingResponse { get; set; }
}
