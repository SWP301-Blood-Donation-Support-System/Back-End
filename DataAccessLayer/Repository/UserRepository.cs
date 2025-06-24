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

        // Ghi chú: T?t c? các b? l?c `IsDeleted` và `IsActive` ?ã ???c xóa nh? Global Filter.
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
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
                .Where(u => u.NextEligibleDonationDate <= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<bool> UpdateDonationInfoAsync(int userId, DateTime donationDate)
        {
            // S?A: Dùng FirstOrDefaultAsync ?? tuân th? Global Filter
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return false;
            }

            user.LastDonationDate = donationDate;
            user.NextEligibleDonationDate = donationDate.AddMonths(3);
            //user.DonationCount++;
            user.UpdatedAt = DateTime.UtcNow;

            // GHI CHÚ: Logic g?i UpdateUserDonationAvailabilityAsync nên ???c chuy?n lên Service Layer.
            // await UpdateUserDonationAvailabilityAsync(userId, 2); // Dòng này nên n?m ? Service

            _context.Users.Update(user);
            return true;
        }

        public async Task<bool> UpdateUserDonationAvailabilityAsync(int userId, int donationAvailabilityId)
        {
            // S?A: Dùng FirstOrDefaultAsync ?? tuân th? Global Filter
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return false;
            }

            user.DonationAvailabilityId = donationAvailabilityId;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
        {
            // S?A: Dùng FirstOrDefaultAsync ?? tuân th? Global Filter
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
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