using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;

namespace BusinessLayer.IService
{
    public interface IDonationRegistrationServices
    {
        Task<DonationRegistration> GetRegistrationByIdAsync(int registrationId);
        Task<IEnumerable<DonationRegistration>> GetAllRegistrationsAsync();
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByDonorIdAsync(int donorId);
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByScheduleIdAsync(int scheduleId);
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByStatusIdAsync(int statusId);
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByQrCodeAsync(string qrCode);
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByTimeSlotIdAsync(int timeSlotId);
        Task AddRegistrationAsync(DonationRegistrationDTO registration);
        Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId);
        Task<bool> SoftDeleteRegistrationAsync(int registrationId); 
        Task<bool> SaveChangesAsync();
    }
}
