using System.ComponentModel.DataAnnotations;

namespace Command.Models;

public class Commands
{
    [Key]
    [Required]
    public Guid Id {get; set;}

    [Required]
    public string HowTo {get; set;} = string.Empty;

    [Required]
    public string CommandLine {get; set;} = string.Empty;

    [Required]
    public Guid PlatformId {get; set;}
    
    public Platform? Platform {get; set;}
}