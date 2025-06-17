using System;
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
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByScheduleId(int scheduleId)
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
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationByTimeSlotIdAsync(int timeSlotId)
        {
            return await _context.DonationRegistrations.Where(r => r.TimeSlotId == timeSlotId && !r.IsDeleted).ToListAsync();
        }
        public async Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId)
        {
            var registration = await _context.DonationRegistrations.FindAsync(registrationId);
            if (registration == null)
            {
                return false;
            }
            registration.RegistrationStatusId = statusId;
            registration.UpdatedAt = DateTime.UtcNow;
            _context.DonationRegistrations.Update(registration);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<IEnumerable<DonationRegistration>> GetRegistrationsByScheduleIdAsync(int scheduleId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByTimeSlotIdAsync(int timeSlotId)
        {
            var registrations = _context.DonationRegistrations
                .Where(r => r.TimeSlotId == timeSlotId && !r.IsDeleted)
                .ToListAsync();
          return await registrations;
        }

        public async Task<bool> SoftDeleteRegistrationAsync(int registrationId)
        {
            var registration = await _context.DonationRegistrations.FindAsync(registrationId);
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

       
    }
}
