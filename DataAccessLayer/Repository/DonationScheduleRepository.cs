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
        private readonly BloodDonationDbContext _context;
        private const int DefaultMaxCapacity = 100;

        public DonationScheduleRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonationSchedule>> GetUpcomingSchedules()
        {
            var yesterday = DateTime.Now.Date.AddDays(-1);
            // Không cần `!s.IsDeleted` nữa nhờ Global Filter
            return await _context.DonationSchedules
                                 .Where(s => s.ScheduleDate > yesterday)
                                 .OrderBy(s => s.ScheduleDate)
                                 .ToListAsync();
        }

        // SỬA: Đổi tên phương thức theo chuẩn PascalCase
        public async Task<DonationSchedule> GetScheduleByDateAsync(DateOnly date)
        {
            // Không cần `!s.IsDeleted` nữa nhờ Global Filter
            return await _context.DonationSchedules
                                 .FirstOrDefaultAsync(s => s.ScheduleDate == date.ToDateTime(TimeOnly.MinValue));
        }

        public async Task<DonationSchedule> GetScheduleWithRegistrationsID(int scheduleId)
        {
            // Global Filter cho DonationRegistration sẽ tự động lọc collection được include (từ EF Core 5+)
            return await _context.DonationSchedules
                                 .Include(s => s.DonationRegistrations)
                                 .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
        }

        public async Task<bool> SoftDeleteSchedule(int scheduleId, string deletedBy)
        {
            // Dùng FirstOrDefaultAsync để tuân thủ Global Filter
            var schedule = await _context.DonationSchedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            if (schedule == null)
            {
                return false;
            }

            schedule.IsDeleted = true;
            schedule.UpdatedBy = deletedBy;
            schedule.UpdatedAt = DateTime.Now;

            _context.DonationSchedules.Update(schedule);
            return true; // Xóa SaveChangesAsync()
        }

        public async Task<bool> RestoreSchedule(int scheduleId, string restoredBy)
        {
            // Để khôi phục, ta cần tìm cả schedule đã bị xóa, nên phải bỏ qua Global Filter
            var schedule = await _context.DonationSchedules
                                         .IgnoreQueryFilters()
                                         .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            if (schedule == null)
            {
                return false;
            }

            schedule.IsDeleted = false;
            schedule.UpdatedBy = restoredBy;
            schedule.UpdatedAt = DateTime.Now;

            _context.DonationSchedules.Update(schedule);
            return true; // Xóa SaveChangesAsync()
        }

        public async Task<bool> UpdateRegisteredSlots(int scheduleId, int change)
        {

            var schedule = await _context.DonationSchedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null) return false;

            if (schedule.RegisteredSlots + change < 0) return false;

            var maxCapacity = await GetMaxCapacity(scheduleId);
            if (schedule.RegisteredSlots + change > maxCapacity) return false;

            schedule.RegisteredSlots += change;
            schedule.UpdatedAt = DateTime.Now;

            return true; // Xóa SaveChangesAsync()

        }

        public async Task<int> GetRegisteredSlotsCount(int scheduleId)
        {
            var schedule = await _context.DonationSchedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            return schedule?.RegisteredSlots ?? 0;
        }

        public async Task<int> GetMaxCapacity(int scheduleId)
        {
            var schedule = await _context.DonationSchedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
            return schedule != null ? DefaultMaxCapacity : 0;
        }
        public async Task<DonationSchedule> GetLatestScheduleAsync()
        {
            return await _context.DonationSchedules
                                 .OrderByDescending(s => s.ScheduleDate)
                                 .FirstOrDefaultAsync();
        }
    }
}