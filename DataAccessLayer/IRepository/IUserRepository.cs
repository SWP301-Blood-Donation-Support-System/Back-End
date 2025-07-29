using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByNationalIdAsync(string nationalId);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<User>> GetByRoleIdAsync(int roleId);
        Task<IEnumerable<User>> GetByBloodTypeIdAsync(int bloodTypeId);
        Task<IEnumerable<User>> GetEligibleDonorsAsync();
        Task<bool> UpdateDonationInfoAsync(int userId, DateTime donationDate);
        Task<bool> UpdateUserDonationAvailabilityAsync(int userId, int donationAvailabilityId );
        Task<bool> UpdateUserRoleAsync(int userId, int roleId);
        Task<User> GetByPasswordResetToken(string token);
        /// <summary>
        /// L?y danh s�ch ng??i c� th? hi?n m�u trong X ng�y t?i
        /// </summary>
        /// <param name="daysAhead">S? ng�y t?i (m?c ??nh 3 ng�y)</param>
        /// <returns>Danh s�ch user c� th? hi?n m�u</returns>
        Task<IEnumerable<User>> GetUpcomingEligibleDonorsAsync(int daysAhead = 3);
    }
}