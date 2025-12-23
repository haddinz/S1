namespace Command.Dtos;

public class CommandsReadDto
{
    public Guid Id { get; set; }
    public string HowTo { get; set; } = string.Empty;
    public string CommandLine { get; set; } = string.Empty;
    public Guid PlatformId { get; set; }
}
