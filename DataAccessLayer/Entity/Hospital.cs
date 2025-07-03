using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class Hospital
{
    public int HospitalId { get; set; }

    public string HospitalName { get; set; } = null!;
    public string? HospitalAddress { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }



    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
