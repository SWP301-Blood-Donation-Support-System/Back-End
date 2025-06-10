using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationAvailability
{
    public int AvailabilityId { get; set; }

    public string AvailabilityName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
