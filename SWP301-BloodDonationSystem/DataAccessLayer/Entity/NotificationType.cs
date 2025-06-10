using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class NotificationType
{
    public int NotificationTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
