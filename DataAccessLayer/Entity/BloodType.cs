using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodType
{
    public int BloodTypeId { get; set; }

    public string BloodTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<BloodCompatibility> BloodCompatibilityDonorBloodTypes { get; set; } = new List<BloodCompatibility>();

    public virtual ICollection<BloodCompatibility> BloodCompatibilityRecipientBloodTypes { get; set; } = new List<BloodCompatibility>();

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
