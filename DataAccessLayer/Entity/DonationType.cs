using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationType
{
    public int DonationTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public int? MinWaitTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<DonationRecord> DonationRecords { get; set; } = new List<DonationRecord>();
}
