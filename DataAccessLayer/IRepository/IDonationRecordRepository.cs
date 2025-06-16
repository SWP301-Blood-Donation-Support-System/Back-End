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
        Task<IEnumerable<DonationRecord>> GetRecordsByDonationDateTimeAsync(DateTime donationDateTime);
        Task<IEnumerable<DonationRecord>> GetRecordsByDonationTypeIdAsync(int donationTypeId);
        Task<IEnumerable<DonationRecord>> GetRecordsByResultAsync(int result);
        Task<bool> UpdateDonationRecordAsync(int recordId, DonationRecord updatedRecord);
        Task<bool> AddDonationValidationAsync(DonationValidation validation);
        Task<bool> AddDonationValidationAsync(int donationRecordId, int userId);
        Task<IEnumerable<DonationValidation>> GetValidationsForRecordAsync(int recordId);
        Task<bool> RemoveValidationAsync(int validationId);
        Task<bool> HasValidationAsync(int recordId, int userId);
        Task<IEnumerable<DonationRecord>> GetRecordsByValidatorAsync(int userId);
    }
}
