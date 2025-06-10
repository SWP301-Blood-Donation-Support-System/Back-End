using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class TimeSlot
{
    public int SlotId { get; set; }

    public string? SlotName { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
