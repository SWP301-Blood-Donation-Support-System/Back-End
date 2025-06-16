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
    public class TimeSlotRepository : GenericRepository<TimeSlot>, ITimeSlotRepository
    {
        private readonly BloodDonationDbContext _context;
        public TimeSlotRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync()
        {
            return await _context.TimeSlots
                .Where(ts => !ts.IsDeleted).ToListAsync();
        }

        public async Task<TimeSlot> GetTimeSlotByIdAsync(int timeSlotId)
        { 
            return await _context.TimeSlots.Where(ts=>ts.TimeSlotId==timeSlotId && !ts.IsDeleted)
                .FirstOrDefaultAsync();
        }
    }
}
