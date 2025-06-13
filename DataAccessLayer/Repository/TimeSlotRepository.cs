using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
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

    }
}
