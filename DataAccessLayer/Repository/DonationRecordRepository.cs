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

        // === CÁC PHƯƠNG THỨC LẤY DỮ LIỆU ĐÃ ĐƯỢC THÊM BỘ LỌC ISDELETED ===

        public async Task<IEnumerable<DonationRecord>> GetRecordsByRegistrationIdAsync(int registrationId)
        {
            return await _context.DonationRecords
                .Where(r => r.RegistrationId == registrationId && !r.IsDeleted) // THÊM: Lọc bản ghi đã xóa
                .ToListAsync();
        }

        public new async Task<DonationRecord> GetByIdAsync(int recordId)
        {
            return await _context.DonationRecords
                .FirstOrDefaultAsync(r => r.DonationRecordId == recordId && !r.IsDeleted); // THÊM: Lọc bản ghi đã xóa
        }

        public async Task<DonationRecord> GetRecordByIdAsync(int recordId)
        {
            return await GetByIdAsync(recordId);
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonationDateTimeAsync(DateTime donationDateTime)
        {
            return await _context.DonationRecords
                .Where(r => r.DonationDateTime.Date == donationDateTime.Date && !r.IsDeleted) // THÊM: Lọc bản ghi đã xóa
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByDonationTypeIdAsync(int donationTypeId)
        {
            return await _context.DonationRecords
                .Where(r => r.DonationTypeId == donationTypeId && !r.IsDeleted) // THÊM: Lọc bản ghi đã xóa
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByResultAsync(int result)
        {
            return await _context.DonationRecords
                .Where(r => r.BloodTestResult == result && !r.IsDeleted) // THÊM: Lọc bản ghi đã xóa
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetAllAsync()
        {
            return await _context.DonationRecords
                .Where(r => !r.IsDeleted) // THÊM: Lọc bản ghi đã xóa
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetAllRecordsWithDonor()
        {
            return await _context.DonationRecords
                .Include(d=>d.DonationType)
                .Include(r => r.Registration)
                .ThenInclude(reg => reg.Donor)
                .ThenInclude(bl=>bl.BloodType)
                .Where(r => !r.IsDeleted) 
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByValidatorAsync(int userId)
        {
            var validatedRecordIds = await _context.DonationValidations
                .Where(v => v.UserId == userId && !v.IsDeleted) // THÊM: Lọc validation đã xóa
                .Select(v => v.DonationRecordId)
                .Distinct()
                .ToListAsync();

            return await _context.DonationRecords
                .Where(r => validatedRecordIds.Contains(r.DonationRecordId) && !r.IsDeleted) // THÊM: Lọc bản ghi đã xóa
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRecord>> GetRecordsByUserIdAsync(int userId)
        {
            return await _context.DonationRecords
                .Where(dr => dr.Registration.Donor.UserId == userId && !dr.IsDeleted) // THÊM: Lọc bản ghi đã xóa
                .Include(dr => dr.Registration)
                .ToListAsync();
        }

        // === CÁC PHƯƠNG THỨC THAY ĐỔI DỮ LIỆU ===

        // THÊM: Logic cho việc xóa mềm (soft delete)
        public async Task<bool> DeleteAsync(int recordId)
        {
            var record = await _context.DonationRecords
                .FirstOrDefaultAsync(r => r.DonationRecordId == recordId && !r.IsDeleted);

            if (record == null)
            {
                return false; // Không tìm thấy hoặc đã bị xóa
            }

            // Đánh dấu là đã xóa
            record.IsDeleted = true;
            record.UpdatedAt = DateTime.Now;

            _context.DonationRecords.Update(record);
            // SaveChangesAsync sẽ được gọi từ Service như code gốc của bạn
            return true;
        }

        public async Task<bool> UpdateAsync(DonationRecord donationRecord)
        {
            if (donationRecord == null)
                throw new ArgumentNullException(nameof(donationRecord));

            var existingRecord = await _context.DonationRecords
                .FirstOrDefaultAsync(r => r.DonationRecordId == donationRecord.DonationRecordId && !r.IsDeleted); // THÊM: Lọc bản ghi đã xóa

            if (existingRecord == null)
                return false;

            _context.Entry(existingRecord).CurrentValues.SetValues(donationRecord);
            existingRecord.UpdatedAt = DateTime.Now;

            return true;
        }

        public async Task<bool> UpdateDonationRecordAsync(int recordId, DonationRecord updatedRecord)
        {
            if (updatedRecord == null) throw new ArgumentNullException(nameof(updatedRecord));
            if (recordId <= 0) throw new ArgumentOutOfRangeException(nameof(recordId));

            var existingRecord = await _context.DonationRecords
                .FirstOrDefaultAsync(r => r.DonationRecordId == recordId && !r.IsDeleted); // THÊM: Lọc bản ghi đã xóa

            if (existingRecord == null) return false;

            updatedRecord.DonationRecordId = recordId;
            _context.Entry(existingRecord).CurrentValues.SetValues(updatedRecord);
            existingRecord.UpdatedAt = DateTime.Now;

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

        // === CÁC PHƯƠNG THỨC LIÊN QUAN ĐẾN VALIDATION ===

        public async Task<bool> AddDonationValidationAsync(DonationValidation validation)
        {
            await _context.DonationValidations.AddAsync(validation);
            return true;
        }

        public async Task<bool> AddDonationValidationAsync(int donationRecordId, int userId)
        {
            var validation = new DonationValidation
            {
                DonationRecordId = donationRecordId,
                UserId = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };
            await _context.DonationValidations.AddAsync(validation);
            return true;
        }

        public async Task<IEnumerable<DonationValidation>> GetValidationsForRecordAsync(int recordId)
        {
            return await _context.DonationValidations
                .Where(v => v.DonationRecordId == recordId && !v.IsDeleted) // THÊM: Lọc validation đã xóa
                .Include(v => v.User)
                .ToListAsync();
        }

        public async Task<bool> RemoveValidationAsync(int validationId)
        {
            var validation = await _context.DonationValidations
                .FirstOrDefaultAsync(v => v.ValidationId == validationId && !v.IsDeleted); // THÊM: Lọc validation đã xóa

            if (validation == null) return false;

            validation.IsDeleted = true;
            validation.UpdatedAt = DateTime.Now;
            return true;
        }

        public async Task<bool> HasValidationAsync(int recordId, int userId)
        {
            return await _context.DonationValidations
                .AnyAsync(v => v.DonationRecordId == recordId && v.UserId == userId && !v.IsDeleted); // THÊM: Lọc validation đã xóa
        }

        public async Task<DonationRecord> GetRecordAndRegistrationAndUserAsync(int recordId)
        {
            return await _context.DonationRecords
                .Include(r => r.Registration)
                .ThenInclude(d => d.Donor)
                .FirstOrDefaultAsync(r => r.DonationRecordId == recordId && !r.IsDeleted);
        }
    }
}