using System;

namespace DataAccessLayer.DTO
{
    public class DonationRecordUpdateDTO
    {
        public int RecordId { get; set; }
        public DateTime? DonationDateTime { get; set; }
        public decimal? DonorWeight { get; set; }
        public decimal? DonorTemperature { get; set; }
        public string? DonorBloodPressure { get; set; }
        public int? DonationTypeId { get; set; }
        public decimal? VolumeDonated { get; set; }
        public string? Note { get; set; }
        public int? BloodTestResult { get; set; }
    }
}