using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IDonationScheduleService
    {
        Task<IEnumerable<DonationSchedule>> GetAllDonationSchedulesAsync();
        Task<DonationSchedule> GetDonationScheduleByIdAsync(int id);
        Task<DonationSchedule> CreateDonationScheduleAsync(DonationSchedule schedule, string createdBy);
        Task<bool> UpdateDonationScheduleAsync(DonationSchedule schedule, string updatedBy);
        Task<bool> DeleteDonationScheduleAsync(int id, string deletedBy); // Chú ý đổi tên phương thức
        Task<bool> SoftDeleteDonationScheduleAsync(int id, string deletedBy); // Giữ lại xóa mềm

        // Business-specific Operations
        Task<IEnumerable<DonationSchedule>> GetUpcomingAvailableDonationSchedulesAsync();
        Task<DonationSchedule> GetDonationScheduleWithRegistrationsAndDetailsAsync(int scheduleId);
        Task<bool> RegisterForDonationSlotAsync(int scheduleId, string registeredBy);
        Task<bool> IsDonationScheduleFullyBookedAsync(int scheduleId);
        Task<bool> RestoreDonationScheduleAsync(int scheduleId, string restoredBy);
    }
}
