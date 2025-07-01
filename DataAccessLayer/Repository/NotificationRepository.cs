using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly BloodDonationDbContext _context;
        public NotificationRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Notification> AddAsync(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));
            await _context.Notifications.AddAsync(notification);
            return notification;
        }
        public async Task<IEnumerable<Notification>> GetNotificationByIdAsync(int notiId)
        {
            return await _context.Notifications
                .Where(n => n.NotificationId == notiId && !n.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<Notification>> GetNotificationByTypeIdAsync(int notiTypeId)
        {
            return await _context.Notifications
                .Where(n => n.NotificationTypeId == notiTypeId && !n.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .Where(n => !n.IsDeleted)
                .ToListAsync();
        }
        public async Task<bool> SoftDeleteNotificationAsync(int notiId)
        {
            var notification = await _context.Notifications.FindAsync(notiId);
            if (notification == null)
            {
                return false; // Notification not found
            }
            notification.IsDeleted = true; // Soft delete
            _context.Notifications.Update(notification);
            return await _context.SaveChangesAsync() > 0; // Save changes
        }
        public async Task<IEnumerable<Notification>> GetAllNotificationAsync()
        {
            return await _context.Notifications
                .Where(n => !n.IsDeleted)
                .ToListAsync();
        }
    
    public async Task<bool> UpdateNotificationAsync(Notification notification)
        {
            var existingNotification = await _context.Notifications.FindAsync(notification.NotificationId);
            if (existingNotification == null)
            {
                return false;
            }
            existingNotification.NotificationId = notification.NotificationId;
            existingNotification.Subject = notification.Subject;
            existingNotification.Message = notification.Message;
            existingNotification.NotificationTypeId = notification.NotificationTypeId;
            _context.Notifications.Update(existingNotification);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}