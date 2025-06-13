using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entity;

namespace DataAccessLayer.IRepository
{
    public interface IDonationRecordRepository : IGenericRepository<DonationRecord>
    {
        Task<DonationRecord> GetRecordByIdAsync(int recordId);
        Task<IEnumerable<DonationRecord>> GetRecordsByRegistrationIdAsync(int registrationId);
        Task<IEnumerable<DonationRecord>> GetRecordsByDonationDateTimeIdAsync(DateTime donationDateTime);
        Task<IEnumerable<DonationRecord>> GetRecordsByDonorWeightAsync(int donorWeight);
        Task<IEnumerable<DonationRecord>> GetRecordsByDonorTemperatureAsync(decimal donorTemperature);
        Task<IEnumerable<DonationRecord>> GetDonationRecordsByBloodTypeIdAsync(int bloodTypeId);
        Task<IEnumerable<DonationRecord>> GetRecordsByBloodPressureAsync(string bloodPressure);
        Task<IEnumerable<DonationRecord>> GetRecordsByVolumeDonatedAsync(decimal volumeDonated);
        Task<IEnumerable<DonationRecord>> GetRecordsByNoteAsync(string note);
        Task<IEnumerable<DonationRecord>> GetRecordByResultAsync(int result);
        Task<bool> UpdateDonationRecordAsync(int recordId, DonationRecord updatedRecord);

    }
}
