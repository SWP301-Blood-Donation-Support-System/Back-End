using BusinessLayer.IService;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class DonationScheduleService : IDonationScheduleService
    {
        private readonly IDonationScheduleRepository _donationScheduleRepository;

        public DonationScheduleService(IDonationScheduleRepository donationScheduleRepository)
        {
            _donationScheduleRepository = donationScheduleRepository;
        }

        public async Task<IEnumerable<DonationSchedule>> GetAllDonationSchedulesAsync()
        {
            return await _donationScheduleRepository.GetAllAsync();
        }
        public async Task<DonationSchedule> GetDonationSchedulesByDateAsync(DateOnly date)
        {
            if (date == default)
            {
                throw new ArgumentException("Date cannot be default", nameof(date));
            }
            return await _donationScheduleRepository.getSchedulebyDateAsync(date);
        }

        public async Task<DonationSchedule> GetDonationScheduleByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
            }
            return await _donationScheduleRepository.GetByIdAsync(id);
        }

        public async Task<DonationSchedule> CreateDonationScheduleAsync(DonationSchedule schedule, string createdBy)
        {
            if (schedule == null)
            {
                throw new ArgumentNullException(nameof(schedule), "Schedule cannot be null");
            }
            if (string.IsNullOrWhiteSpace(createdBy))
            {
                throw new ArgumentException("Created by cannot be null or empty", nameof(createdBy));
            }
            schedule.CreatedBy = createdBy;
            schedule.CreatedAt = DateTime.UtcNow;
            var result = await _donationScheduleRepository.AddAsync(schedule);
            await _donationScheduleRepository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> UpdateDonationScheduleAsync(DonationSchedule schedule, string updatedBy)
        {
            if (schedule == null)
            {
                throw new ArgumentNullException(nameof(schedule), "Schedule cannot be null");
            }
            if (string.IsNullOrWhiteSpace(updatedBy))
            {
                throw new ArgumentException("Updated by cannot be null or empty", nameof(updatedBy));
            }
            schedule.UpdatedBy = updatedBy;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _donationScheduleRepository.UpdateAsync(schedule);
            return await _donationScheduleRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteDonationScheduleAsync(int id, string deletedBy)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Deleted by cannot be null or empty", nameof(deletedBy));
            }

            var result = await _donationScheduleRepository.DeleteAsync(id);
            return await _donationScheduleRepository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteDonationScheduleAsync(int id, string deletedBy)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
            }
            if (string.IsNullOrWhiteSpace(deletedBy))
            {
                throw new ArgumentException("Deleted by cannot be null or empty", nameof(deletedBy));
            }

            var schedule = await _donationScheduleRepository.GetByIdAsync(id);
            if (schedule == null)
            {
                return false;
            }

            schedule.IsDeleted = true;
            schedule.UpdatedBy = deletedBy;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _donationScheduleRepository.UpdateAsync(schedule);
            return await _donationScheduleRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<DonationSchedule>> GetUpcomingAvailableDonationSchedulesAsync()
        {
            return await _donationScheduleRepository.GetUpcomingSchedules();
        }

        public async Task<DonationSchedule> GetDonationScheduleWithRegistrationsAndDetailsAsync(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scheduleId), "Schedule ID must be greater than zero");
            }

            return await _donationScheduleRepository.GetScheduleWithRegistrationsID(scheduleId);
        }

        public async Task<bool> RegisterForDonationSlotAsync(int scheduleId, string registeredBy)
        {
            if (scheduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scheduleId), "Schedule ID must be greater than zero");
            }
            if (string.IsNullOrWhiteSpace(registeredBy))
            {
                throw new ArgumentException("Registered by cannot be null or empty", nameof(registeredBy));
            }

            var schedule = await _donationScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null || schedule.IsDeleted)
            {
                return false;
            }

            // Implement business logic for registering for a donation slot
            // This would typically involve creating a DonationRegistration record
            // through a separate repository and service

            // For now, just increment the registered slots count
            schedule.RegisteredSlots++;
            schedule.UpdatedBy = registeredBy;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _donationScheduleRepository.UpdateAsync(schedule);
            return await _donationScheduleRepository.SaveChangesAsync();
        }

        public async Task<bool> IsDonationScheduleFullyBookedAsync(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scheduleId), "Schedule ID must be greater than zero");
            }

            var schedule = await _donationScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null || schedule.IsDeleted)
            {
                // Consider a non-existent or deleted schedule as "fully booked"
                return true;
            }

            // Implement logic to determine if a schedule is fully booked
            // This might involve comparing RegisteredSlots with a MaxSlots property
            // For now, assuming a fixed capacity of 100 slots per schedule
            const int maxSlotsPerSchedule = 100;
            return schedule.RegisteredSlots >= maxSlotsPerSchedule;
        }

        public async Task<bool> RestoreDonationScheduleAsync(int scheduleId, string restoredBy)
        {
            if (scheduleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scheduleId), "Schedule ID must be greater than zero");
            }
            if (string.IsNullOrWhiteSpace(restoredBy))
            {
                throw new ArgumentException("Restored by cannot be null or empty", nameof(restoredBy));
            }

            var schedule = await _donationScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
            {
                return false;
            }

            if (!schedule.IsDeleted)
            {
                // Schedule is not deleted, no need to restore
                return true;
            }

            schedule.IsDeleted = false;
            schedule.UpdatedBy = restoredBy;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _donationScheduleRepository.UpdateAsync(schedule);
            return await _donationScheduleRepository.SaveChangesAsync();
        }
    }
}
