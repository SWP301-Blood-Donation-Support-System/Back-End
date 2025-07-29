using BusinessLayer.IService;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly INotificationRepository _notificationRepository;
        public UserNotificationService(IUserNotificationRepository userNotificationRepository, INotificationRepository notificationRepository)
        {
            _userNotificationRepository = userNotificationRepository;
            _notificationRepository = notificationRepository;
        }
        public async Task CreateNotificationForUserAsync(int recipientId, string subject, string message, int notificationTypeId)
        {
            // 1. Tạo đối tượng Notification
            var notification = new Notification
            {
                NotificationTypeId = notificationTypeId,
                Subject = subject,
                Message = message,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync(); // Lưu để lấy NotificationId

            // 2. Tạo UserNotification để liên kết
            var userNotification = new UserNotification
            {
                RecipientId = recipientId,
                NotificationId = notification.NotificationId,
                IsRead = false,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            await _userNotificationRepository.AddAsync(userNotification);
            await _userNotificationRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserNotification>> GetUnreadNotificationsAsync(int recipientId)
        {
            return await _userNotificationRepository.GetUnreadNotificationsByRecipientIdAsync(recipientId); // Giả định có phương thức này
        }

        public async Task<bool> MarkNotificationAsReadAsync(int userNotificationId)
        {
            var userNotification = await _userNotificationRepository.GetByIdAsync(userNotificationId);
            if (userNotification == null || userNotification.IsRead)
            {
                return false;
            }

            userNotification.IsRead = true;
            userNotification.DeliveredAt = DateTime.Now;
            await _userNotificationRepository.UpdateAsync(userNotification);
            return await _userNotificationRepository.SaveChangesAsync();
        }
    }
}
