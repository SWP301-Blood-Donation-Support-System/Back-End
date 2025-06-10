using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodUnit
{
    public int BloodUnitId { get; set; }

    public int DonationRecordId { get; set; }

    public int BloodTypeId { get; set; }

    public int? ComponentId { get; set; }

    public DateTime? CollectedDateTime { get; set; }

    public DateTime? ExpiryDateTime { get; set; }

    public decimal? Volume { get; set; }

    public int? BloodUnitStatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual BloodType BloodType { get; set; } = null!;

    public virtual BloodUnitStatus? BloodUnitStatus { get; set; }

    public virtual BloodComponent? Component { get; set; }

    public virtual DonationRecord DonationRecord { get; set; } = null!;
}
