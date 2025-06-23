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
    public class BloodUnitRepository: GenericRepository<BloodUnit>, IBloodUnitRepository
    {
        private readonly BloodDonationDbContext _context;
        public BloodUnitRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BloodUnit>> GetUnitsByBloodComponentIdAsync(int bloodComponentId)
        {
            return await _context.BloodUnits
                .Where(p=>p.ComponentId==bloodComponentId).ToListAsync();
        }

        public async Task<IEnumerable<BloodUnit>> GetUnitsByBloodTypeIdAsync(int bloodTypeId)
        {
            return await _context.BloodUnits
                .Where(p=>p.BloodTypeId==bloodTypeId).ToListAsync();
        }

        public async Task<IEnumerable<BloodUnit>> GetUnitsByRecordIdAsync(int recordId)
        {
            return await _context.BloodUnits
                .Where(p=>p.DonationRecordId==recordId).ToListAsync();
        }

        public async Task<IEnumerable<BloodUnit>> GetUnitsByStatusAsync(int bloodUnitStatusId)
        {
            return await _context.BloodUnits
                .Where(p => p.BloodUnitStatusId == bloodUnitStatusId).ToListAsync();
        }

        public Task<bool> UpdateUnitStatusAsync(int unitId, int bloodUnitStatusId)
        {
            var unit = _context.BloodUnits.Find(unitId);
            if (unit == null)
            {
                return Task.FromResult(false);
            }
            unit.BloodUnitStatusId = bloodUnitStatusId;
            _context.BloodUnits.Update(unit);
            return Task.FromResult(true);
        }

        // Use 'new' keyword instead of 'override'
        public new async Task<IEnumerable<BloodUnit>> GetAllAsync()
        {
            return await _context.BloodUnits
                .Include(b => b.DonationRecord)
                    .ThenInclude(dr => dr.Registration)
                        .ThenInclude(r => r.Donor)
                .Include(b => b.BloodType)
                .Include(b => b.Component)
                .Include(b => b.BloodUnitStatus)
                .ToListAsync();
        }

        // Use 'new' keyword instead of 'override'
        public new async Task<BloodUnit> GetByIdAsync(int id)
        {
            return await _context.BloodUnits
                .Include(b => b.DonationRecord)
                    .ThenInclude(dr => dr.Registration)
                        .ThenInclude(r => r.Donor)
                .Include(b => b.BloodType)
                .Include(b => b.Component)
                .Include(b => b.BloodUnitStatus)
                .FirstOrDefaultAsync(b => b.BloodUnitId == id);
        }
    }
}
