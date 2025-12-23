using System.ComponentModel.DataAnnotations;

namespace Command.Dtos;

public class CommandsCreateDto
{
    [Required]
    public string HowTo { get; set; } = string.Empty;

    [Required]
    public string CommandLine { get; set; } = string.Empty;
}
