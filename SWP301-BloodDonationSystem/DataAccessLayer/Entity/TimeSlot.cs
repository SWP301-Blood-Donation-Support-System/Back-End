using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class TimeSlot
{
    public int SlotId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ICollection<DonationAppointment> DonationAppointments { get; set; } = new List<DonationAppointment>();
}
