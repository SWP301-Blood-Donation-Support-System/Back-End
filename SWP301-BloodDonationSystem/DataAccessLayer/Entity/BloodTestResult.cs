using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodTestResult
{
    public int ResultId { get; set; }

    public string? ResultName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<DonationRecord> DonationRecords { get; set; } = new List<DonationRecord>();
}
