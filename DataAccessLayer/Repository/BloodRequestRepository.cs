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
    public class BloodRequestRepository : GenericRepository<BloodRequest>,
        IBloodRequestRepository
    {
        private readonly BloodDonationDbContext _context;
        public BloodRequestRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByBloodTypeIdAsync(int bloodTypeId)
        {
            return await _context.BloodRequests
                .Where(r => r.BloodTypeId == bloodTypeId)
                .Include(r => r.RequestStatus)
                .Include(r => r.Urgency)
                .Include(r => r.RequestingStaff)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByComponentIdAsync(int componentId)
        {
            return await _context.BloodRequests
                .Where(r => r.BloodComponentId == componentId)
                .Include(r => r.RequestStatus)
                .Include(r => r.Urgency)
                .Include(r => r.RequestingStaff)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByStaffIdAsync(int staffId)
        {
            return await _context.BloodRequests
                .Where(r => r.RequestingStaffId == staffId)
                .Include(r => r.RequestStatus)
                .Include(r => r.Urgency)
                .Include(r => r.RequestingStaff)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByStatusIdAsync(int statusId)
        {
            return await _context.BloodRequests
                .Where(r => r.RequestStatusId == statusId)
                .Include(r => r.RequestStatus)
                .Include(r => r.Urgency)
                .Include(r => r.RequestingStaff)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetBloodRequestsByUrgencyIdAsync(int urgencyId)
        {
            return await _context.BloodRequests
                .Where(r => r.UrgencyId == urgencyId)
                .Include(r => r.RequestStatus)
                .Include(r => r.Urgency)
                .Include(r => r.RequestingStaff)
                .ToListAsync();
        }

        public async Task<BloodRequest> GetBloodRequestWithDetailsAsync(int id)
        {
            return await _context.BloodRequests
                .Include(r => r.BloodType)
                .Include(r => r.BloodComponent)
                .Include(r => r.RequestStatus)
                .Include(r => r.Urgency)
                .Include(r => r.RequestingStaff)
                .FirstOrDefaultAsync(r => r.RequestId == id);
        }

        public async Task<bool> UpdateBloodRequestStatusAsync(int requestId, int statusId)
        {
            var request = await _context.BloodRequests.FindAsync(requestId);
            if (request == null)
            {
                return false; 
            }
            request.RequestStatusId = statusId;
            request.UpdatedAt= DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SoftDeleteBloodRequestAsync(int requestId)
        {
            var request= await _context.BloodRequests.FindAsync(requestId);
            if (request == null)
            {
                return false;
            }
            request.IsDeleted= true;
            request.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
