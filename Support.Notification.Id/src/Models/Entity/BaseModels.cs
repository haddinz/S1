using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Support.Notification.Id.Models.Entity;

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
