namespace Support.Auth.Id.Domain.ValueObject;

public sealed class FrontendSettings
{
    public const string SectionName = "Frontend";
    public string BaseUrl { get; init; } = string.Empty;
}