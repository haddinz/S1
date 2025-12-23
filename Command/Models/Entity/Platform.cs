using System.ComponentModel.DataAnnotations;

namespace Command.Models;

public class Platform
{
    [Key]
    [Required]
    public Guid Id {get; set;}

    [Required]
    public Guid ExternalId {get; set;}

    [Required]
    public string Name {get; set;} = string.Empty;
    
    public ICollection<Commands> Commands = new List<Commands>();
}