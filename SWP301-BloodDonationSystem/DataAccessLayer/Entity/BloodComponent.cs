using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodComponent
{
    public int ComponentId { get; set; }

    public string ComponentName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();
}
