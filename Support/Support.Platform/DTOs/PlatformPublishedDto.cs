using Support.Platform.Models.Enum;

namespace Support.Platform.DTO;

public class PlatformPublishedDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Published Event;
}
