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
        void SendWelcomeEmail(string userEmail, string fullName = "");
        void SendDonationRegistrationThankYouEmail(string userEmail, string fullName, DonationRegistrationEmailInfoDTO registrationInfo);
        Task<byte[]> GetUserImageAsync(int userId);
        Task<bool> DeleteUserImageAsync(int userId);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<bool> ChangePasswordAsync(int userId,string currentPassword, string newPassword);
        
        // NEW METHODS FOR DONATION REMINDER FEATURE
        /// <summary>
        /// L?y danh sách ng??i có th? hi?n máu trong X ngày t?i
        /// </summary>
        /// <param name="daysAhead">S? ngày t?i</param>
        /// <returns>Danh sách UpcomingEligibleDonorsDTO</returns>
        Task<IEnumerable<UpcomingEligibleDonorsDTO>> GetUpcomingEligibleDonorsAsync(int daysAhead = 3);
        
        /// <summary>
        /// G?i thông báo nh?c nh? hàng lo?t cho danh sách user
        /// </summary>
        /// <param name="request">Thông tin request</param>
        /// <param name="adminUserId">ID c?a admin th?c hi?n</param>
        /// <returns>K?t qu? g?i thông báo</returns>
        Task<BulkReminderResponseDTO> SendBulkDonationRemindersAsync(BulkReminderRequestDTO request, int adminUserId);

        // NEW METHODS FOR TOMORROW DONATION REMINDER FEATURE
        /// <summary>
        /// L?y danh sách ng??i có l?ch hi?n máu vào ngày mai
        /// </summary>
        /// <returns>Danh sách TomorrowDonationScheduleDTO</returns>
        Task<IEnumerable<TomorrowDonationScheduleDTO>> GetTomorrowDonationSchedulesAsync();

        /// <summary>
        /// G?i thông báo nh?c nh? t? ??ng cho nh?ng ng??i có l?ch hi?n vào ngày mai
        /// </summary>
        /// <returns>K?t qu? x? lý</returns>
        Task<AutoReminderJobResponseDTO> SendTomorrowDonationRemindersAsync();
    }
}