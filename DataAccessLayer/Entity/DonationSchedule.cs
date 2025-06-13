using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationSchedule
{
    public int ScheduleId { get; set; }
 
    public DateTime? ScheduleDate { get; set; }

    public int RegisteredSlots { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<DonationRegistration> DonationRegistrations { get; set; } = new List<DonationRegistration>();
}
