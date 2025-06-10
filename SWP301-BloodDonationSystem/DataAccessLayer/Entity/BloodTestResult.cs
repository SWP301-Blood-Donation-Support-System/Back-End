using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodTestResult
{
    public int ResultId { get; set; }

    public string? ResultName { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<DonationRecord> DonationRecords { get; set; } = new List<DonationRecord>();
}
