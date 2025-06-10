using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class RegistrationStatus
{
    public int RegistrationStatusId { get; set; }

    public string RegistrationStatusName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
