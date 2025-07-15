using BusinessLayer.IService;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using DataAccessLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BusinessLayer.Service
{
    public class DonationScheduleService : IDonationScheduleService
    {
        private readonly IDonationScheduleRepository _donationScheduleRepository;
        private readonly BloodDonationDbContext _context;


        // SỬA: Xóa bỏ sự phụ thuộc vào DbContext
        public DonationScheduleService(IDonationScheduleRepository donationScheduleRepository)
        {

            _donationScheduleRepository = donationScheduleRepository ?? throw new ArgumentNullException(nameof(donationScheduleRepository));

        }

        public async Task<IEnumerable<DonationSchedule>> GetAllDonationSchedulesAsync()
        {
            return await _donationScheduleRepository.GetAllAsync();
        }

        public async Task<DonationSchedule> GetDonationScheduleByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
            return await _donationScheduleRepository.GetByIdAsync(id);
        }

        public async Task<DonationSchedule> CreateDonationScheduleAsync(DonationSchedule schedule, string createdBy)
        {
            if (schedule == null) throw new ArgumentNullException(nameof(schedule));
            if (string.IsNullOrWhiteSpace(createdBy)) throw new ArgumentException("Created by cannot be null or empty", nameof(createdBy));

            schedule.CreatedBy = createdBy;
            schedule.CreatedAt = DateTime.UtcNow;

            var result = await _donationScheduleRepository.AddAsync(schedule);
            await _donationScheduleRepository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> UpdateDonationScheduleAsync(DonationSchedule schedule, string updatedBy)
        {
            if (schedule == null) throw new ArgumentNullException(nameof(schedule));
            if (string.IsNullOrWhiteSpace(updatedBy)) throw new ArgumentException("Updated by cannot be null or empty", nameof(updatedBy));

            var existingSchedule = await _donationScheduleRepository.GetByIdAsync(schedule.ScheduleId);
            if (existingSchedule == null) return false;

            // Cập nhật các thuộc tính
            existingSchedule.ScheduleDate = schedule.ScheduleDate;
            // ... các thuộc tính khác
            existingSchedule.UpdatedBy = updatedBy;
            existingSchedule.UpdatedAt = DateTime.UtcNow;

            await _donationScheduleRepository.UpdateAsync(existingSchedule);
            return await _donationScheduleRepository.SaveChangesAsync();
        }

        // SỬA: Gộp lại còn một phương thức Delete duy nhất
        public async Task<bool> DeleteDonationScheduleAsync(int id, string deletedBy)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(deletedBy)) throw new ArgumentException("Deleted by cannot be null or empty", nameof(deletedBy));

            // Gọi đến phương thức xóa mềm trong repository
            var result = await _donationScheduleRepository.SoftDeleteSchedule(id, deletedBy);
            if (!result) return false; // Không tìm thấy schedule để xóa

            return await _donationScheduleRepository.SaveChangesAsync();
        }


        // SỬA: Logic khôi phục đã đúng
        public async Task<bool> RestoreDonationScheduleAsync(int scheduleId, string restoredBy)
        {
            if (scheduleId <= 0) throw new ArgumentOutOfRangeException(nameof(scheduleId));
            if (string.IsNullOrWhiteSpace(restoredBy)) throw new ArgumentException("Restored by cannot be null or empty", nameof(restoredBy));

            // Gọi đến phương thức restore của repository
            var result = await _donationScheduleRepository.RestoreSchedule(scheduleId, restoredBy);
            if (!result) return false; // Không tìm thấy schedule để khôi phục

            return await _donationScheduleRepository.SaveChangesAsync();

        }
        public async Task<IEnumerable<DonationSchedule>> GetUpcomingAvailableDonationSchedulesAsync()
        {
            return await _donationScheduleRepository.GetUpcomingSchedules();
        }

        public async Task<DonationSchedule> GetDonationScheduleWithRegistrationsAndDetailsAsync(int scheduleId)
        {
            if (scheduleId <= 0) throw new ArgumentOutOfRangeException(nameof(scheduleId));
            return await _donationScheduleRepository.GetScheduleWithRegistrationsID(scheduleId);
        }

        public async Task<bool> RegisterForDonationSlotAsync(int scheduleId, string registeredBy)
        {
            if (scheduleId <= 0) throw new ArgumentOutOfRangeException(nameof(scheduleId));
            if (string.IsNullOrWhiteSpace(registeredBy)) throw new ArgumentException("Registered by cannot be null or empty", nameof(registeredBy));


            // SỬA: Gọi đến phương thức đã được sửa trong repository
            var result = await _donationScheduleRepository.UpdateRegisteredSlots(scheduleId, 1);
            if (!result) return false;

            // SỬA: Phải lưu thay đổi lại
            return await _donationScheduleRepository.SaveChangesAsync();

        }


        public async Task<bool> IsDonationScheduleFullyBookedAsync(int scheduleId)
        {
            if (scheduleId <= 0) throw new ArgumentOutOfRangeException(nameof(scheduleId));

            var schedule = await _donationScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null) return true; // Coi như đã đầy

            // Ghi chú: maxSlotsPerSchedule nên là một thuộc tính của schedule
            const int maxSlotsPerSchedule = 100;
            return schedule.RegisteredSlots >= maxSlotsPerSchedule;
        }
        public async Task<DonationSchedule> GetDonationSchedulesByDateAsync(DateOnly date)
        {

            if (date == default)
            {
                throw new ArgumentException("Date cannot be default", nameof(date));
            }
            return await _donationScheduleRepository.GetScheduleByDateAsync(date);
        }

        public async Task CheckAndCreateSchedulesIfNeededAsync()
        {
            const int thresholdDays = 14; // Nếu lịch trống trong vòng 14 ngày tới
            const int createForwardDays = 30; // Thì tạo lịch cho 30 ngày tiếp theo
            const string systemUser = "System-AutoJob"; // Người tạo

            var latestSchedule = await _donationScheduleRepository.GetLatestScheduleAsync();
            var today = DateTime.UtcNow.Date;

            // Xác định ngày cuối cùng có lịch. Nếu không có lịch nào, bắt đầu từ hôm nay.
            var lastScheduledDate = latestSchedule?.ScheduleDate?.Date ?? today;

            // Kiểm tra xem có cần tạo lịch mới không.
            // Điều kiện: Ngày cuối cùng có lịch cách hôm nay dưới 14 ngày.
            if (lastScheduledDate < today.AddDays(thresholdDays))
            {
                // Ngày bắt đầu tạo lịch mới là ngày sau ngày cuối cùng đã có lịch
                var startDate = lastScheduledDate.AddDays(1);
                var endDate = startDate.AddDays(createForwardDays);

                var newSchedules = new List<DonationSchedule>();

                for (var day = startDate; day <= endDate; day = day.AddDays(1))
                {

                        newSchedules.Add(new DonationSchedule
                        {
                            ScheduleDate = day,
                            RegisteredSlots = 0,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = systemUser,
                            IsDeleted = false
                        });
                    
                }

                if (newSchedules.Any())
                {
                    foreach (var schedule in newSchedules)
                    {
                        await _donationScheduleRepository.AddAsync(schedule);
                    }
                    await _donationScheduleRepository.SaveChangesAsync();
                }
            }
        }
    }
}