using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class Occupation
{
    public int OccupationId { get; set; }

    public string OccupationName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
