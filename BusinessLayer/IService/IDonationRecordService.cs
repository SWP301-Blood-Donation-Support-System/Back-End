using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entity;

namespace BusinessLayer.IService
{
    public interface IDonationRecordService
    {
        Task<IEnumerable<DonationRecord>> GetAllDonationRecordsAsync();
        Task<DonationRecord> GetDonationRecordByIdAsync(int recordId);
        Task<DonationRecord> AddDonationRecordAsync(DonationRecord donationRecord);
        Task<bool> UpdateDonationRecordAsync(DonationRecord record);
        Task<bool> DeleteDonationRecordAsync(int recordId);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByRegistrationIdAsync(int registrationId);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByDonationDateTimeAsync(DateTime donationDateTime);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByDonorWeightAsync(int donorWeight);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByDonorTemperatureAsync(decimal donorTemperature);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByDonationTypeIdAsync(int donationTypeId);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByNoteAsync(string note);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByBloodPressureAsync(string bloodPressure);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByVolumeDonatedAsync(decimal volumeDonated);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByResultAsync(int result);
        Task<bool> SaveChanges();
    }
}
