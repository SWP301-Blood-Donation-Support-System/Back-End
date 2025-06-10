using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class Occupation
{
    public int OccupationId { get; set; }

    public string OccupationName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
