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
            return await _context.Users
                .Include(u => u.BloodType)
                .Where(u => u.BloodTypeId == bloodTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetEligibleDonorsAsync()
        {
            return await _context.Users
                .Where(u => u.DonationAvailabilityId == 1 && u.RoleId==3)
                .ToListAsync();
        }

        /// <summary>
        /// L?y danh sách ng??i có th? hi?n máu trong X ngày t?i
        /// </summary>
        /// <param name="daysAhead">S? ngày t?i</param>
        /// <returns>Danh sách user có th? hi?n máu</returns>
        public async Task<IEnumerable<User>> GetUpcomingEligibleDonorsAsync(int daysAhead = 3)
        {
            var today = DateTime.Now.Date;
            var endDate = today.AddDays(daysAhead);

            return await _context.Users
                .Include(u => u.BloodType)
                .Where(u => 
                    u.RoleId == 3 && // Ch? l?y donor (role 3)
                    u.IsActive && 
                    !u.IsDeleted &&
                    u.NextEligibleDonationDate.HasValue &&
                    u.NextEligibleDonationDate.Value.Date >= today &&
                    u.NextEligibleDonationDate.Value.Date <= endDate)
                .OrderBy(u => u.NextEligibleDonationDate)
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
            user.UpdatedAt = DateTime.Now;

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
            user.UpdatedAt = DateTime.Now;

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
            user.UpdatedAt = DateTime.Now;
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