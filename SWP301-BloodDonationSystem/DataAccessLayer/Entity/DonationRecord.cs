using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationRecord
{
    public int DonationRecordId { get; set; }

    public int RegistrationId { get; set; }

    public DateTime DonationDateTime { get; set; }

    public decimal? DonorWeight { get; set; }

    public decimal? BloodTemperature { get; set; }

    public string? BloodPressure { get; set; }

    public decimal? VolumeExtracted { get; set; }

    public string? Note { get; set; }

    public int? BloodTestResult { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual BloodTestResult? BloodTestResultNavigation { get; set; }

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    public virtual ICollection<DonationValidation> DonationValidations { get; set; } = new List<DonationValidation>();

    public virtual Registration Registration { get; set; } = null!;
}
