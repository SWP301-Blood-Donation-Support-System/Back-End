using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class DonationRecord
{
    public int DonationRecordId { get; set; }

    public int RegistrationId { get; set; }

    public DateTime DonationDateTime { get; set; }

    public decimal? DonorWeight { get; set; }

    public decimal? DonorTemperature { get; set; }

    public string? DonorBloodPressure { get; set; }

    public int? DonationTypeId { get; set; }

    public decimal? VolumeDonated { get; set; }

    public string? Note { get; set; }

    public int? BloodTestResult { get; set; }
    public string? CertificateId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual BloodTestResult? BloodTestResultNavigation { get; set; }

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    public virtual DonationType? DonationType { get; set; }

    public virtual ICollection<DonationValidation> DonationValidations { get; set; } = new List<DonationValidation>();

    public virtual DonationRegistration Registration { get; set; } = null!;

}
