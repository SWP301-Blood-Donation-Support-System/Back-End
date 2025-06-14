using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class DonationRecordRepository : GenericRepository<DonationRecord>, IDonationRecordRepository
    {
        private readonly BloodDonationDbContext _context;

        public DonationRecordRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByRegistrationIdAsync(int registrationId)
        {
            return await _context.DonationRecords
                .Where(r => r.RegistrationId == registrationId && !r.IsDeleted)
                .ToListAsync();
        }


        // Fixed method name to match service expectations
        public async Task<DonationRecord> GetByIdAsync(int recordId)
        {
            return await _context.DonationRecords
                .FirstOrDefaultAsync(r => r.DonationRecordId == recordId && !r.IsDeleted);
        }

        // Keep the original method name as well for backward compatibility
        public async Task<DonationRecord> GetRecordByIdAsync(int recordId)
        {
            return await GetByIdAsync(recordId);
        }

        // Fixed method name to match service expectations
        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonationDateTimeAsync(DateTime donationDateTime)
        {
            return await _context.DonationRecords
                .Where(r => r.DonationDateTime.Date == donationDateTime.Date && !r.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonationTypeIdAsync(int donationTypeId)
        {
            return await _context.DonationRecords
                .Where(r => r.DonationTypeId == donationTypeId && !r.IsDeleted)
                .ToListAsync();
        }

        // Fixed method name to match service expectations
        public async Task<IEnumerable<DonationRecord>> GetRecordsByResultAsync(int result)
        {
            return await _context.DonationRecords
                .Where(r => r.BloodTestResult == result && !r.IsDeleted)
                .ToListAsync();
        }

        // Add missing methods that the service expects
        public async Task<IEnumerable<DonationRecord>> GetAllAsync()
        {
            return await _context.DonationRecords
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }


        public async Task<bool> UpdateAsync(DonationRecord donationRecord)
        {
            if (donationRecord == null)
                throw new ArgumentNullException(nameof(donationRecord));

            var existingRecord = await _context.DonationRecords
                .FirstOrDefaultAsync(r => r.DonationRecordId == donationRecord.DonationRecordId && !r.IsDeleted);

            if (existingRecord == null)
                return false;

            // Update properties
            _context.Entry(existingRecord).CurrentValues.SetValues(donationRecord);
            existingRecord.UpdatedAt = DateTime.UtcNow;

            return true;
        }

        public async Task<bool> UpdateDonationRecordAsync(int recordId, DonationRecord updatedRecord)
        {
            if (updatedRecord == null)
                throw new ArgumentNullException(nameof(updatedRecord), "Updated record cannot be null");

            if (recordId <= 0)
                throw new ArgumentOutOfRangeException(nameof(recordId), "Record ID must be greater than zero");

            var existingRecord = await _context.DonationRecords
                .FirstOrDefaultAsync(r => r.DonationRecordId == recordId && !r.IsDeleted);

            if (existingRecord == null)
                return false;

            // Ensure the ID is preserved
            updatedRecord.DonationRecordId = recordId;

            // Update properties
            _context.Entry(existingRecord).CurrentValues.SetValues(updatedRecord);

            // Set update timestamp
            existingRecord.UpdatedAt = DateTime.UtcNow;

            // Save changes directly in this method
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}