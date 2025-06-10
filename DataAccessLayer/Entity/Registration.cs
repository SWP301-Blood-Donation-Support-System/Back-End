using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class Registration
{
    public int RegistrationId { get; set; }

    public int DonorId { get; set; }

    public int ScheduleId { get; set; }

    public int? TimeSlotId { get; set; }

    public int? DonationTypeId { get; set; }

    public int? RegistrationStatusId { get; set; }

    public string? QrCodeUrl { get; set; }

    public bool IsCheckedIn { get; set; }

    public DateTime RegistrationDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual DonationRecord? DonationRecord { get; set; }

    public virtual DonationType? DonationType { get; set; }

    public virtual User Donor { get; set; } = null!;

    public virtual RegistrationStatus? RegistrationStatus { get; set; }

    public virtual DonationSchedule Schedule { get; set; } = null!;

    public virtual TimeSlot? TimeSlot { get; set; }
}
