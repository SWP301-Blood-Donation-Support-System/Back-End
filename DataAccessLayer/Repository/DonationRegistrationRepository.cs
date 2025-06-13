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
    public class DonationRegistrationRepository : GenericRepository<DonationRegistration>, IDonationRecordRepository
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
        public async Task<IEnumerable<DonationRegistration>> GetRegistrationsByQrCodeAsync(string qrCode)
        {
            return await _context.DonationRegistrations
                .Where(r => r.QrCodeUrl == qrCode && !r.IsDeleted)
                .ToListAsync();
        }
        public async Task<DonationRegistration> GetRegistrationByTimeSlotIdAsync(int timeSlotId)
        {
            return await _context.DonationRegistrations
                .FirstOrDefaultAsync(r => r.TimeSlotId == timeSlotId && !r.IsDeleted);
        }
        public async Task<bool> UpdateRegistrationStatusAsync(int registrationId, int statusId)
        {
            var registration = await _context.DonationRegistrations.FindAsync(registrationId);
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

        public Task<IEnumerable<DonationRegistration>> GetRegistrationsByScheduleIdAsync(int scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DonationRegistration>> GetRegistrationsByTimeSlotIdAsync(int timeSlotId)
        {
            throw new NotImplementedException();
        }
    }
}
