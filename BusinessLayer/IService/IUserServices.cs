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
        Task RegisterHospitalAsync(HospitalRegisterDTO hospital);
        Task<User> UpdateUserAsync(User user);
        Task<User> UpdateDonorAsync(int donorId, DonorDTO donor);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UpdateDonationInfoAsync(int userId, DateTime donationDate);
        Task<bool> UpdateUserDonationAvailabilityAsync(int userId, int donationAvailabilityId);
        Task<bool> UpdateUserRoleAsync(int userId, int roleId);
        Task<bool> UpdateUserBloodTypeAsync(int userId, int bloodTypeId);
        Task<bool> UpdateUserBloodTypeByDonorIdAsync(int donorId, int bloodTypeId);
        Task<bool> SaveChangesAsync();
        Task<string> GenerateToken(LoginDTO login);
        Task<string> ValidateGoogleToken(TokenRequest token);
        void SendMail(string mailSubject, string mailBody, string receiver);
        void SendWelcomeEmail(string userEmail, string userName = "");
        void SendDonationRegistrationThankYouEmail(string userEmail, string userName, DonationRegistrationEmailInfoDTO registrationInfo);
        Task<byte[]> GetUserImageAsync(int userId);
        Task<bool> DeleteUserImageAsync(int userId);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<bool> ChangePasswordAsync(int userId,string currentPassword, string newPassword);
    }
}