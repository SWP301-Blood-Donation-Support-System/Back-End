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

        // Fixed parameter type from int to decimal to match service
        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonorWeightAsync(decimal donorWeight)
        {
            return await _context.DonationRecords
                .Where(r => r.DonorWeight == donorWeight && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonorTemperatureAsync(decimal donorTemperature)
        {
            return await _context.DonationRecords
                .Where(r => r.DonorTemperature == donorTemperature && !r.IsDeleted)
                .ToListAsync();
        }

        // Fixed method name to match service expectations
        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonationTypeIdAsync(int donationTypeId)
        {
            return await _context.DonationRecords
                .Where(r => r.DonationTypeId == donationTypeId && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByBloodPressureAsync(string bloodPressure)
        {
            return await _context.DonationRecords
                .Where(r => r.DonorBloodPressure == bloodPressure && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByVolumeDonatedAsync(decimal volumeDonated)
        {
            return await _context.DonationRecords
                .Where(r => r.VolumeDonated == volumeDonated && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByNoteAsync(string note)
        {
            return await _context.DonationRecords
                .Where(r => r.Note.Contains(note) && !r.IsDeleted)
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

        public async Task<DonationRecord> AddAsync(DonationRecord donationRecord)
        {
            if (donationRecord == null)
                throw new ArgumentNullException(nameof(donationRecord));

            donationRecord.CreatedAt = DateTime.UtcNow;
            donationRecord.UpdatedAt = DateTime.UtcNow;
            donationRecord.IsDeleted = false;

            await _context.DonationRecords.AddAsync(donationRecord);
            return donationRecord;
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

        public async Task<bool> DeleteAsync(int recordId)
        {
            var record = await _context.DonationRecords
                .FirstOrDefaultAsync(r => r.DonationRecordId == recordId && !r.IsDeleted);

            if (record == null)
                return false;

            // Soft delete
            record.IsDeleted = true;
            record.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<IEnumerable<DonationRecord>> GetRecordsByDonationDateTimeIdAsync(DateTime donationDateTime)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DonationRecord>> GetRecordsByDonorWeightAsync(int donorWeight)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DonationRecord>> GetDonationRecordsByBloodTypeIdAsync(int bloodTypeId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DonationRecord>> GetRecordByResultAsync(int result)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateDonationRecordAsync(int recordId, DonationRecord updatedRecord)
        {
            throw new NotImplementedException();
        }
    }
}