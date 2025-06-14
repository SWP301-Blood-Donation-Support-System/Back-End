using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class UserNotification
{
    public int UserNotificationId { get; set; }

    public int RecipientId { get; set; }

    public int NotificationId { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public virtual User Recipient { get; set; } = null!;
}
