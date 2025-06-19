using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entity;

namespace DataAccessLayer.IRepository
{
    public interface IDonationRegistrationRepository : IGenericRepository<DonationRegistration>
    {
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByDonorIdAsync(int donorId);
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByScheduleIdAsync(int scheduleId);
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByStatusIdAsync(int statusId);
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByTimeSlotIdAsync(int timeSlotId);
        Task<DonationRegistration> GetRegistrationWithDonorAndRecordAsync(int registrationId);
        Task<DonationRegistration> GetRegistrationByCertificateIdAsync(string certificateId);
        Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId);
        Task<bool> SoftDeleteRegistrationAsync(int registrationId);
        Task<DonationRegistration?> CheckInByNationalIdAsync(string nationalId, int approvedStatusId, int checkedInStatusId);
        Task<DonationRegistration?> GetByNationalIdAsync(string nationalId);
        Task<IEnumerable<DonationRegistration>> GetByScheduleAndTimeSlotAsync(int scheduleId, int timeSlotId);
        Task<DonationRegistration?> GetTodayRegistrationByNationalIdAsync(string nationalId, int approvedStatusId);

    }
}
