using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationAppointment
{
    public int AppointmentId { get; set; }

    public int DonorId { get; set; }

    public DateTime ScheduledDateTime { get; set; }

    public int? TimeSlotId { get; set; }

    public int? DonationTypeId { get; set; }

    public string? Qrcode { get; set; }

    public string? BookingCode { get; set; }

    public int? AppointmentStatusId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual AppointmentStatus? AppointmentStatus { get; set; }

    public virtual ICollection<DonationRecord> DonationRecords { get; set; } = new List<DonationRecord>();

    public virtual DonationType? DonationType { get; set; }

    public virtual User Donor { get; set; } = null!;

    public virtual TimeSlot? TimeSlot { get; set; }
}
