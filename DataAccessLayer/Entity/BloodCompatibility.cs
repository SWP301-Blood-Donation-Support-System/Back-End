using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodCompatibility
{
    public int CompatibilityId { get; set; }

    public int DonorBloodTypeId { get; set; }

    public int RecipientBloodTypeId { get; set; }

    public bool IsCompatible { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual BloodType DonorBloodType { get; set; } = null!;

    public virtual BloodType RecipientBloodType { get; set; } = null!;
}
