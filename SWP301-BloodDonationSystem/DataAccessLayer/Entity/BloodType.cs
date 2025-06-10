using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodType
{
    public int BloodTypeId { get; set; }

    public string BloodTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
