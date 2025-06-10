using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationType
{
    public int DonationTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<DonationAppointment> DonationAppointments { get; set; } = new List<DonationAppointment>();

    public virtual ICollection<DonationRecord> DonationRecords { get; set; } = new List<DonationRecord>();
}
