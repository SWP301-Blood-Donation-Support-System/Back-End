using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class DonationScheduleRepository : GenericRepository<DonationSchedule>, IDonationScheduleRepository
    {
        private readonly BloodDonationDbContext _context; // Đây là DbContext của bạn
        private const int DefaultMaxCapacity = 100; // Default max capacity for schedules

        public DonationScheduleRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context; // Gán context cho biến cục bộ
        }

        public async Task<IEnumerable<DonationSchedule>> GetUpcomingSchedules()
        {
            return await _context.DonationSchedules
                                 .Where(s => s.ScheduleDate >= DateTime.UtcNow && !s.IsDeleted)
                                 .OrderBy(s => s.ScheduleDate)
                                 .ToListAsync();
        }
        public async Task<DonationSchedule> getSchedulebyDateAsync(DateOnly date)
        {
            return await _context.DonationSchedules
                                 .FirstOrDefaultAsync(s => s.ScheduleDate == date.ToDateTime(TimeOnly.MinValue) && !s.IsDeleted);

        }

        public async Task<DonationSchedule> GetScheduleWithRegistrationsID(int scheduleId)
        {
            return await _context.DonationSchedules
                                 .Include(s => s.DonationRegistrations)
                                 .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId && !s.IsDeleted);
        }

        public async Task<bool> SoftDeleteSchedule(int scheduleId, string deletedBy)
        {
            var schedule = await _context.DonationSchedules.FindAsync(scheduleId);
            if (schedule == null)
            {
                return false;
            }

            schedule.IsDeleted = true;
            schedule.UpdatedBy = deletedBy;
            schedule.UpdatedAt = DateTime.UtcNow;

            _context.DonationSchedules.Update(schedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RestoreSchedule(int scheduleId, string restoredBy)
        {
            var schedule = await _context.DonationSchedules.FindAsync(scheduleId);
            if (schedule == null)
            {
                return false;
            }

            schedule.IsDeleted = false;
            schedule.UpdatedBy = restoredBy;
            schedule.UpdatedAt = DateTime.UtcNow;

            _context.DonationSchedules.Update(schedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateRegisteredSlots(int scheduleId, int change)
        {
            var schedule = await _context.DonationSchedules.FindAsync(scheduleId);
            if (schedule == null)
            {
                return false;
            }

            // Prevent negative values
            if (schedule.RegisteredSlots + change < 0)
            {
                return false;
            }

            // Check against max capacity
            var maxCapacity = await GetMaxCapacity(scheduleId);
            if (schedule.RegisteredSlots + change > maxCapacity)
            {
                return false;
            }

            schedule.RegisteredSlots += change;
            schedule.UpdatedAt = DateTime.UtcNow;

            _context.DonationSchedules.Update(schedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetRegisteredSlotsCount(int scheduleId)
        {
            var schedule = await _context.DonationSchedules.FindAsync(scheduleId);
            return schedule?.RegisteredSlots ?? 0;
        }

        public async Task<int> GetMaxCapacity(int scheduleId)
        {
            // In a real-world scenario, this might be a property of the DonationSchedule entity
            // For now, using a constant value
            var schedule = await _context.DonationSchedules.FindAsync(scheduleId);
            return schedule != null ? DefaultMaxCapacity : 0;
        }

       
    }
}
