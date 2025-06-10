using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodRequestStatus
{
    public int BloodRequestStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
}
