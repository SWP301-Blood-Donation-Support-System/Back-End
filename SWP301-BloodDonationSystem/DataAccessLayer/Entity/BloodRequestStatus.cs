using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodRequestStatus
{
    public int BloodRequestStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
}
