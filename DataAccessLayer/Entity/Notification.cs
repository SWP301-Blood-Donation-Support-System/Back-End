using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int RecipientId { get; set; }

    public int NotificationTypeId { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }

    public DateTime? SentDateTime { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual NotificationType NotificationType { get; set; } = null!;

    public virtual User Recipient { get; set; } = null!;
}
