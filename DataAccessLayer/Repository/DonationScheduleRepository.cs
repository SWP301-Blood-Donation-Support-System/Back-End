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

        public async Task<DonationSchedule> GetScheduleWithRegistrationsID(int scheduleId)
        {
            return await _context.DonationSchedules
                                 .Include(s => s.DonationRegistrations)
                                 .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId && !s.IsDeleted);
        }



    }
}
