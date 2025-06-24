using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationRegistration
{
    public int RegistrationId { get; set; }

    public int DonorId { get; set; }

    public int ScheduleId { get; set; }

    public int? TimeSlotId { get; set; }

    public int? RegistrationStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual DonationRecord? DonationRecord { get; set; }

    public virtual User Donor { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual RegistrationStatus? RegistrationStatus { get; set; }

    public virtual DonationSchedule Schedule { get; set; } = null!;

    public virtual TimeSlot? TimeSlot { get; set; }
}
