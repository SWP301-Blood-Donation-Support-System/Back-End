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
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByQrCodeAsync(string qrCode);
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByTimeSlotIdAsync(int timeSlotId);
        Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId);
    }
}
