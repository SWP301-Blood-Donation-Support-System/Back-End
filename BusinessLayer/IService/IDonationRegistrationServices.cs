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
        Task<IEnumerable<DonationRegistration>> GetRegistrationsByTimeSlotIdAsync(int timeSlotId);
        Task AddRegistrationAsync(DonationRegistrationDTO registration);
        Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId);
        Task<bool> SoftDeleteRegistrationAsync(int registrationId); 
        Task<bool> SaveChangesAsync();
        Task<DonationRegistration?> CheckInByNationalIdAsync(string nationalId, int approvedStatusId, int checkedInStatusId);
        Task<IEnumerable<DonationRegistration>> GetByScheduleAndTimeSlotAsync(int scheduleId, int timeSlotId);
        Task<DonationRegistration?> GetTodayRegistrationByNationalIdAsync(string nationalId, int approvedStatusId);

        // Add the DTO-based methods to prevent circular references
        Task<IEnumerable<DonationRegistrationResponseDTO>> GetAllRegistrationsResponseAsync();
        Task<DonationRegistrationResponseDTO> GetRegistrationByIdResponseAsync(int registrationId);
        Task<IEnumerable<DonationRegistrationResponseDTO>> GetRegistrationsByDonorIdResponseAsync(int donorId);
        Task<DonationRegistrationResponseDTO> CheckInByNationalIdResponseAsync(string nationalId, int approvedStatusId, int checkedInStatusId);
        Task<IEnumerable<DonationRegistrationResponseDTO>> GetRegistrationsByScheduleIdResponseAsync(int scheduleId);
        Task<IEnumerable<DonationRegistrationResponseDTO>> GetRegistrationsByStatusIdResponseAsync(int statusId);
        Task<IEnumerable<DonationRegistrationResponseDTO>> GetRegistrationsByTimeSlotIdResponseAsync(int timeSlotId);
        Task<IEnumerable<DonationRegistrationResponseDTO>> GetByScheduleAndTimeSlotResponseAsync(int scheduleId, int timeSlotId);
    }
}
