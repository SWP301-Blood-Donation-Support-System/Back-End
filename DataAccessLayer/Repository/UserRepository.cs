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

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }
        public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId)
        {

            return await _context.Users
                .Where(u => u.RoleId == roleId && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByBloodTypeIdAsync(int bloodTypeId)
        {

            return await _context.Users
                .Where(u => u.BloodTypeId == bloodTypeId && u.IsActive && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetEligibleDonorsAsync()
        {
            return await _context.Users
                .Where(u => u.NextEligibleDonationDate <= DateTime.UtcNow
                         && u.IsActive
                         && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> UpdateDonationInfoAsync(int userId, DateTime donationDate)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.LastDonationDate = donationDate;
            user.NextEligibleDonationDate = donationDate.AddMonths(3);
            user.DonationCount++;
            await UpdateUserDonationAvailabilityAsync(userId, 2);
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            return true;
        }

        public async Task<bool> UpdateUserDonationAvailabilityAsync(int userId, int donationAvailabililtyId)
        {

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.DonationAvailabilityId = donationAvailabililtyId;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            return true;
        }
        public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.RoleId = roleId;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            return true;
        }
    }
}