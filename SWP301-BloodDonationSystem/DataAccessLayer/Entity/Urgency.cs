using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class Urgency
{
    public int UrgencyId { get; set; }

    public string UrgencyName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
}
