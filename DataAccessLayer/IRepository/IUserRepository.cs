using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByNationalIdAsync(string nationalId);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<User>> GetByRoleIdAsync(int roleId);
        Task<IEnumerable<User>> GetByBloodTypeIdAsync(int bloodTypeId);
        Task<IEnumerable<User>> GetEligibleDonorsAsync();
        Task<bool> UpdateDonationInfoAsync(int userId, DateTime donationDate);
        Task<bool> UpdateUserDonationAvailabilityAsync(int userId, int donationAvailabilityId );
        Task<bool> UpdateUserRoleAsync(int userId, int roleId);
    }
}