﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class DonationRegistrationRepository : GenericRepository<DonationRegistration>, IDonationRegistrationRepository
    {
        private readonly BloodDonationDbContext _context;

        public DonationRegistrationRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByDonorIdAsync(int donorId)
        {
            return await _context.DonationRegistrations
                .Where(r => r.DonorId == donorId && !r.IsDeleted)
                .ToListAsync();
        }

        // SỬA: Gộp các phương thức GetRegistrationsByScheduleIdAsync bị trùng lặp
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByScheduleIdAsync(int scheduleId)
        {
            return await _context.DonationRegistrations
                .Where(r => r.ScheduleId == scheduleId && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByStatusIdAsync(int statusId)
        {
            return await _context.DonationRegistrations
                .Where(r => r.RegistrationStatusId == statusId && !r.IsDeleted)
                .ToListAsync();
        }

        // SỬA: Gộp các phương thức Get...ByTimeSlotIdAsync bị trùng lặp và đổi tên cho nhất quán
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByTimeSlotIdAsync(int timeSlotId)
        {
            return await _context.DonationRegistrations
                .Where(r => r.TimeSlotId == timeSlotId && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId)
        {
            // FindAsync sẽ tìm cả những record đã bị soft-delete, nên cần kiểm tra thêm
            var registration = await _context.DonationRegistrations.FindAsync(registrationId);


            // THÊM: Kiểm tra xem bản ghi có tồn tại và chưa bị xóa không

            if (registration == null || registration.IsDeleted)
            {
                return false;
            }

            registration.RegistrationStatusId = statusId;
            registration.UpdatedAt = DateTime.UtcNow;
            _context.DonationRegistrations.Update(registration);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteRegistrationAsync(int registrationId)
        {
            var registration = await _context.DonationRegistrations.FindAsync(registrationId);

            // Logic này đã đúng, nếu không tìm thấy hoặc đã xóa rồi thì trả về false
            if (registration == null || registration.IsDeleted)
            {
                return false;
            }

            registration.IsDeleted = true;
            registration.UpdatedAt = DateTime.UtcNow;
            _context.DonationRegistrations.Update(registration);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<DonationRegistration?> CheckInByNationalIdAsync(string nationalId, int approvedStatusId, int checkedInStatusId)
        {
            var today = DateTime.Today;

            var registration = await _context.DonationRegistrations
                .Include(r => r.Donor)
                .Include(r => r.Schedule)
                .FirstOrDefaultAsync(r =>
                    r.Donor.NationalId == nationalId &&
                    r.Schedule.ScheduleDate.Value.Date == today &&
                    r.RegistrationStatusId == approvedStatusId); // Dùng tham số approvedStatusId

            if (registration == null)
            {
                return null;
            }

            // Cập nhật trạng thái bằng tham số checkedInStatusId
            registration.RegistrationStatusId = checkedInStatusId;
            registration.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return registration;
        }

        public async Task<DonationRegistration?> GetByNationalIdAsync(string nationalId)
        {
            return await _context.DonationRegistrations
                .Include(r => r.Donor)
                .FirstOrDefaultAsync(r => r.Donor.NationalId == nationalId && !r.IsDeleted);
        }
        public async Task<IEnumerable<DonationRegistration>> GetByScheduleAndTimeSlotAsync(int scheduleId, int timeSlotId)
        {
            return await _context.DonationRegistrations
                .Where(r => r.ScheduleId == scheduleId && r.TimeSlotId == timeSlotId)
                .Include(r => r.Donor) // Lấy luôn thông tin người hiến máu
                .ToListAsync();
        }

        public async Task<DonationRegistration?> GetTodayRegistrationByNationalIdAsync(string nationalId, int approvedStatusId)
{
    var today = DateTime.Today;

    return await _context.DonationRegistrations
        .Include(r => r.Donor)
        .Include(r => r.Schedule)
        .Include(r => r.TimeSlot) // Include TimeSlot để hiển thị thông tin ca làm việc
        .FirstOrDefaultAsync(r =>
            r.Donor.NationalId == nationalId &&
            r.Schedule.ScheduleDate.Value.Date == today &&
            r.RegistrationStatusId == approvedStatusId &&
            !r.IsDeleted);
}

        public async Task<DonationRegistration> GetRegistrationWithDonorAndRecordAsync(int registrationId)
        {
            var registration =  _context.DonationRegistrations
                .Include(r => r.Donor)
                .ThenInclude(d=>d.BloodType)
                .Include(r => r.DonationRecord)
                .FirstOrDefaultAsync(r => r.RegistrationId == registrationId && !r.IsDeleted);
            return await registration;
        }

        public Task<DonationRegistration> GetRegistrationByCertificateIdAsync(string certificateId)
        {
            var registration = _context.DonationRegistrations
                .Include(r => r.Donor)
                .ThenInclude(d => d.BloodType)  
                .Include(r => r.DonationRecord)
                .FirstOrDefaultAsync(r => r.DonationRecord.CertificateId == certificateId && !r.IsDeleted);
            return registration;

        }

    }
}