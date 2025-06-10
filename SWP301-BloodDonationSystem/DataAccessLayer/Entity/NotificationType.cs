using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class NotificationType
{
    public int NotificationTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
