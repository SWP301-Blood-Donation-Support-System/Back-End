﻿using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class BloodRequest
{
    public int RequestId { get; set; }

    public int RequestingStaffId { get; set; }

    public int BloodTypeId { get; set; }

    public int BloodComponentId { get; set; }

    public decimal Volume { get; set; }

    public DateTime? RequestDateTime { get; set; }

    public DateTime? RequiredDateTime { get; set; }

    public int? RequestStatusId { get; set; }

    public int? UrgencyId { get; set; }

    public string? Note { get; set; }

    public int? ApprovedByUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual BloodComponent BloodComponent { get; set; } = null!;

    public virtual BloodType BloodType { get; set; } = null!;

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    public virtual BloodRequestStatus? RequestStatus { get; set; }

    public virtual User RequestingStaff { get; set; } = null!;

    public virtual Urgency? Urgency { get; set; }
}
