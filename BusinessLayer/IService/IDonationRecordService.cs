using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BusinessLayer.IService
{
    public interface IDonationRecordService
    {
        Task<IEnumerable<DonationRecord>> GetAllDonationRecordsAsync();
        Task<DonationRecord> GetRecordsByIdAsync(int recordId);
        Task AddRecordsAsync(DonationRecordDTO donationRecord);
        Task<bool> UpdateRecordsAsync(DonationRecord record);
        Task<bool> DeleteRecordsAsync(int recordId);
        Task<IEnumerable<DonationRecord>> GetRecordsByRegistrationIdAsync(int registrationId);
        Task<IEnumerable<DonationRecord>> GetRecordsByDonationDateTimeAsync(DateTime donationDateTime);
        Task<IEnumerable<DonationRecord>> GetRecordsByDonationTypeIdAsync(int donationTypeId);
        Task<IEnumerable<DonationRecord>> GetRecordsByResultAsync(int result);
        Task<bool> SaveChanges();
    }
}
