using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationValidation
{
    public int ValidationId { get; set; }

    public int UserId { get; set; }

    public int DonationRecordId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual DonationRecord DonationRecord { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
