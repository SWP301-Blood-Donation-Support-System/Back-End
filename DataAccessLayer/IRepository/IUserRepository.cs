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
        /// L?y danh sách ng??i có th? hi?n máu trong X ngày t?i
        /// </summary>
        /// <param name="daysAhead">S? ngày t?i (m?c ??nh 3 ngày)</param>
        /// <returns>Danh sách user có th? hi?n máu</returns>
        Task<IEnumerable<User>> GetUpcomingEligibleDonorsAsync(int daysAhead = 3);
    }
}