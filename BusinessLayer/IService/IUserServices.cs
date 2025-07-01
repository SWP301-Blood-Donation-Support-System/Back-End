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
        Task RegisterStaffAsync(StaffRegisterDTO staff);
        Task RegisterAdminAsync(StaffRegisterDTO admin);
        Task<User> UpdateUserAsync(User user);
        Task<User> UpdateDonorAsync(int donorId, DonorDTO donor);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UpdateDonationInfoAsync(int userId, DateTime donationDate);
        Task<bool> UpdateUserDonationAvailabilityAsync(int userId, int donationAvailabilityId);
        Task<bool> UpdateUserRoleAsync(int userId, int roleId);
        Task<bool> SaveChangesAsync();
        Task<string> GenerateToken(LoginDTO login);
        Task<string> ValidateGoogleToken(TokenRequest token);
        void SendMail(string mailSubject, string mailBody, string receiver);
        void SendWelcomeEmail(string userEmail, string userName = "");
        Task<byte[]> GetUserImageAsync(int userId);
        Task<bool> DeleteUserImageAsync(int userId);
    }
}