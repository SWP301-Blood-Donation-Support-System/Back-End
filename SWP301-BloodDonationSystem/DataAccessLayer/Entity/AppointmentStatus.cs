using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class AppointmentStatus
{
    public int AppointmentStatusId { get; set; }

    public string AppointmentStatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<DonationAppointment> DonationAppointments { get; set; } = new List<DonationAppointment>();
}
