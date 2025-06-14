using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int NotificationTypeId { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual NotificationType NotificationType { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
