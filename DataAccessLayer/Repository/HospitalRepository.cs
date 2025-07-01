using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class HospitalRepository : GenericRepository<Hospital>, IHospitalRepository
    {
        private readonly BloodDonationDbContext _context;
        public HospitalRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> SoftDeleteHospitalAsync(int hospitalId)
        {
            var request= await _context.Hospitals.FindAsync(hospitalId);
            if (request == null)
            {
                return false; // Hospital not found
            }
            request.IsDeleted = true; // Mark as deleted
           request.UpdatedAt = DateTime.UtcNow; // Update the timestamp
           await _context.SaveChangesAsync();
            return true;

        }
    }
}
