namespace Support.Auth.Id.Models.Dtos.Request;

public class TokenRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
