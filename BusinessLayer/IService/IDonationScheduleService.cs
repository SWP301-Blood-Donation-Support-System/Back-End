using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IDonationScheduleService
    {
        Task<IEnumerable<DonationSchedule>> GetAllDonationSchedulesAsync();
        Task<DonationSchedule> GetDonationScheduleByIdAsync(int id);
        Task<DonationSchedule> CreateDonationScheduleAsync(DonationSchedule schedule, string createdBy);
        Task<bool> UpdateDonationScheduleAsync(DonationSchedule schedule, string updatedBy);

        // SỬA: Bỏ phương thức SoftDelete... thừa, chỉ giữ lại một phương thức Delete duy nhất.
        Task<bool> DeleteDonationScheduleAsync(int id, string deletedBy);

        Task<bool> RestoreDonationScheduleAsync(int scheduleId, string restoredBy);

        // Giữ lại phương thức này vì nó cần thiết
        Task<DonationSchedule> GetDonationSchedulesByDateAsync(DateOnly date);

        // Business-specific Operations
        Task<IEnumerable<DonationSchedule>> GetUpcomingAvailableDonationSchedulesAsync();
        Task<DonationSchedule> GetDonationScheduleWithRegistrationsAndDetailsAsync(int scheduleId);
        Task<bool> RegisterForDonationSlotAsync(int scheduleId, string registeredBy);
        Task<bool> IsDonationScheduleFullyBookedAsync(int scheduleId);
    }
}