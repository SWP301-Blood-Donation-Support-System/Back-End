using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationRecord
{
    public int DonationRecordId { get; set; }

    public int AppointmentId { get; set; }

    public int DonorId { get; set; }

    public int? ProcessingStaffId { get; set; }

    public DateTime DonationDateTime { get; set; }

    public decimal? DonorWeight { get; set; }

    public decimal? BloodTemperature { get; set; }

    public string? BloodPressure { get; set; }

    public int? DonationTypeId { get; set; }

    public decimal? VolumeExtracted { get; set; }

    public string? Note { get; set; }

    public int? BloodTestResult { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual DonationAppointment Appointment { get; set; } = null!;

    public virtual BloodTestResult? BloodTestResultNavigation { get; set; }

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    public virtual DonationType? DonationType { get; set; }

    public virtual User Donor { get; set; } = null!;

    public virtual User? ProcessingStaff { get; set; }
}
