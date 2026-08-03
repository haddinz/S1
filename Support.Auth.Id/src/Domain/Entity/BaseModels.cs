using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Id.Domain.Entity;

public class BaseModels
{
    [Key]
    public Guid Id { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdateAt { get; private set; }

    public bool IsDeleted { get; private set; } = false;

    protected BaseModels() { }

    protected void SetUpdate() => UpdateAt = DateTime.UtcNow;

    public void SetSoftDelete()
    {
        IsDeleted = true;
        SetUpdate();
    }
}
