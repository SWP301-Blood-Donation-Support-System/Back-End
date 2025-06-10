using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationAvailability
{
    public int AvailabilityId { get; set; }

    public string AvailabilityName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
