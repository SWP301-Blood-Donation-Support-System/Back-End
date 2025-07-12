using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IDonationScheduleRepository : IGenericRepository<DonationSchedule>
    {
        Task<DonationSchedule> GetLatestScheduleAsync();

        Task<DonationSchedule> GetScheduleByDateAsync(DateOnly date);

        /// <summary>
        /// Gets all upcoming schedules that are not deleted
        /// </summary>
        /// <returns>Collection of upcoming donation schedules</returns>
        Task<IEnumerable<DonationSchedule>> GetUpcomingSchedules();

        /// <summary>
        /// Gets a donation schedule with its registrations
        /// </summary>
        /// <param name="scheduleId">ID of the schedule</param>
        /// <returns>Donation schedule with registrations</returns>
        Task<DonationSchedule> GetScheduleWithRegistrationsID(int scheduleId);

        /// <summary>
        /// Soft deletes a schedule
        /// </summary>
        /// <param name="scheduleId">ID of the schedule</param>
        /// <param name="deletedBy">User who deleted the schedule</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> SoftDeleteSchedule(int scheduleId, string deletedBy);

        /// <summary>
        /// Restores a soft-deleted schedule
        /// </summary>
        /// <param name="scheduleId">ID of the schedule</param>
        /// <param name="restoredBy">User who restored the schedule</param>
        /// <returns>True if restoration was successful</returns>
        Task<bool> RestoreSchedule(int scheduleId, string restoredBy);

        /// <summary>
        /// Updates the number of registered slots for a schedule
        /// </summary>
        /// <param name="scheduleId">ID of the schedule</param>
        /// <param name="change">Change in the number of slots</param>
        /// <returns>True if update was successful</returns>
        Task<bool> UpdateRegisteredSlots(int scheduleId, int change);

        /// <summary>
        /// Gets the count of registered slots for a schedule
        /// </summary>
        /// <param name="scheduleId">ID of the schedule</param>
        /// <returns>Count of registered slots</returns>
        Task<int> GetRegisteredSlotsCount(int scheduleId);

        /// <summary>
        /// Gets the maximum capacity of a schedule
        /// </summary>
        /// <param name="scheduleId">ID of the schedule</param>
        /// <returns>Maximum capacity of the schedule</returns>
        Task<int> GetMaxCapacity(int scheduleId);
    }
}
