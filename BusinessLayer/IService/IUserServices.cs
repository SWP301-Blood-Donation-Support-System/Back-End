using BusinessLayer.Utils;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IUserServices
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
        Task<IEnumerable<User>> GetUsersByBloodTypeAsync(int bloodTypeId);
        Task<IEnumerable<User>> GetEligibleDonorsAsync();
        Task RegisterDonorAsync(RegisterDTO donor);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UpdateDonationInfoAsync(int userId, DateTime donationDate);
        Task<bool> UpdateUserStatusAsync(int userId, bool isActive);
        Task<bool> SaveChangesAsync();
        Task<string> GenerateToken(LoginDTO login);
        Task<string> ValidateGoogleToken(TokenRequest token);
    }
}