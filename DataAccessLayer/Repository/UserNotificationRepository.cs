using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task<UserNotification> AddAsync(UserNotification userNotification)
        {
            if (userNotification == null)
                throw new ArgumentNullException(nameof(userNotification));
            await _context.UserNotifications.AddAsync(userNotification);
            return userNotification;
        }
        public async Task<UserNotification> UpdateAsync(UserNotification userNotification)
        {
            if (userNotification == null)
                throw new ArgumentNullException(nameof(userNotification));
            _context.UserNotifications.Update(userNotification);
            return userNotification;
        }
        public async Task<bool> SoftDeleteNotificationAsync(int unotiId)
        {
            var notification = await _context.UserNotifications.FindAsync(unotiId);
            if (notification == null)
            {
                return false; // Notification not found
            }
            notification.IsDeleted = true; // Soft delete
            _context.UserNotifications.Update(notification);
            return await _context.SaveChangesAsync() > 0; // Save changes
        }
        public async Task<IEnumerable<UserNotification>> GetUserNotificationByNotificationIdAsync(int notiId)
        {
            return await _context.UserNotifications
                .Where(un => un.NotificationId == notiId && !un.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<UserNotification>> GetUserNotificationByRecipientIdAsync(int repiId)
        {
            return await _context.UserNotifications
                .Where(un => un.RecipientId == repiId && !un.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<UserNotification>> GetAllAsync()
        {
            return await _context.UserNotifications
                .Where(un => !un.IsDeleted)
                .ToListAsync();
        }
    }
}
