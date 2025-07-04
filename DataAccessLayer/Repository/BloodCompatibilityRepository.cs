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
    public class BloodCompatibilityRepository : GenericRepository<BloodCompatibility>, IBloodCompatibilityRepository
    {
        private readonly BloodDonationDbContext _context;
        public BloodCompatibilityRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BloodCompatibility>> GetCompatibilityByDonorBloodTypeIdAsync(int bloodTypeId)
        {
            return await _context.BloodCompatibilities
                .Where(bc => bc.DonorBloodTypeId == bloodTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodCompatibility>> GetCompatibilityByRecipientBloodTypeIdAsync(int bloodTypeId)
        {
            return await _context.BloodCompatibilities
                .Where(bc => bc.RecipientBloodTypeId == bloodTypeId)
                .ToListAsync();
        }
        public async Task<bool> IsCompatibleAsync(int donorBloodTypeId, int recipientBloodTypeId)
        {
            return await _context.BloodCompatibilities
                .AnyAsync(bc => bc.DonorBloodTypeId == donorBloodTypeId &&
                               bc.RecipientBloodTypeId == recipientBloodTypeId &&
                               bc.IsCompatible==true);
        }

        public async Task<IEnumerable<int>> GetAllCompatibleDonorBloodTypeIdsAsync(int recipientBloodTypeId)
        {
            return await _context.BloodCompatibilities
                .Where(bc => bc.RecipientBloodTypeId == recipientBloodTypeId && bc.IsCompatible==true)
                .Select(bc => bc.DonorBloodTypeId)
                .Distinct()
                .ToListAsync();
        }
    }
}
