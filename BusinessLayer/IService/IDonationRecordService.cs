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
        Task<bool> ValidateDonationRecordAsync(int recordId, int userId);
        Task<bool> UpdateRecordByFieldsAsync(DonationRecordUpdateDTO updateDTO);
        Task<IEnumerable<DonationRecord>> GetRecordsByResultAsync(int result);
        Task<IEnumerable<DonationRecordDTO>> GetRecordsByUserId(int userId);
        Task<bool> SaveChanges();
        Task<IEnumerable<DonationValidation>> GetValidationsForRecordAsync(int recordId);
        Task<IEnumerable<DonationRecord>> GetRecordsByValidatorAsync(int userId);
    }
}
