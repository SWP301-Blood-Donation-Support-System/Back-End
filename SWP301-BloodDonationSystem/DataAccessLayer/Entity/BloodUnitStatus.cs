using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodUnitStatus
{
    public int BloodUnitStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();
}
