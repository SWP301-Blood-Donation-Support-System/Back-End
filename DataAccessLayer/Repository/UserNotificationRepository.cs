using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class UserNotificationRepository : GenericRepository<UserNotification>, IUserNotificationRepository
    {
        private readonly BloodDonationDbContext _context;

        public UserNotificationRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all notifications for a specific recipient, including the related Notification details.
        /// </summary>
        public async Task<IEnumerable<UserNotification>> GetNotificationsByRecipientIdAsync(int recipientId)
        {
            return await _context.UserNotifications
                .Include(un => un.Notification) // Lấy kèm thông tin chi tiết của Notification
                .Where(un => un.RecipientId == recipientId && !un.IsDeleted)
                .OrderByDescending(un => un.CreatedAt) // Sắp xếp theo ngày tạo mới nhất
                .ToListAsync();
        }

        /// <summary>
        /// Gets all unread notifications for a specific recipient.
        /// </summary>
        public async Task<IEnumerable<UserNotification>> GetUnreadNotificationsByRecipientIdAsync(int recipientId)
        {
            return await _context.UserNotifications
                .Include(un => un.Notification)
                .Where(un => un.RecipientId == recipientId && !un.IsRead && !un.IsDeleted)
                .OrderByDescending(un => un.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Soft deletes a user notification by its ID.
        /// </summary>
        public async Task<bool> SoftDeleteUserNotificationAsync(int userNotificationId)
        {
            var userNotification = await _context.UserNotifications.FindAsync(userNotificationId);

            if (userNotification == null || userNotification.IsDeleted)
            {
                return false; // Không tìm thấy hoặc đã bị xóa
            }

            userNotification.IsDeleted = true;
            userNotification.UpdatedAt = DateTime.Now;

            _context.UserNotifications.Update(userNotification);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}