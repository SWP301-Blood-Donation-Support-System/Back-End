using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly BloodDonationDbContext _context;

        public UserRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }


        public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId)
        {
            return await _context.Users.Where(u => u.RoleId == roleId && !u.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByBloodTypeIdAsync(int bloodTypeId)
        {
            return await _context.Users.Where(u => u.BloodTypeId == bloodTypeId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetEligibleDonorsAsync()
        {
            return await _context.Users
                .Where(u => u.DonationAvailabilityId == 1 && u.RoleId==3)
                .ToListAsync();
        }

        public async Task<bool> UpdateDonationInfoAsync(int userId, DateTime donationDate)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return false;
            }

            user.LastDonationDate = donationDate;
            user.NextEligibleDonationDate = donationDate.AddMonths(3);
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            return true;
        }

        public async Task<bool> UpdateUserDonationAvailabilityAsync(int userId, int donationAvailabilityId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return false;
            }

            user.DonationAvailabilityId = donationAvailabilityId;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return false;
            }
            user.RoleId = roleId;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            _context.SaveChanges();
            return true;
        }

        public async Task<User> GetByNationalIdAsync(string nationalId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.NationalId == nationalId);
        }

        public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
        public async Task<User> GetByPasswordResetToken(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }
    }
}