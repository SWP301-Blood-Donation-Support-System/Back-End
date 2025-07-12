using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IUserNotificationService
    {
        Task CreateNotificationForUserAsync(int recipientId, string subject, string message, int notificationTypeId);
        Task<IEnumerable<UserNotification>> GetUnreadNotificationsAsync(int recipientId);
        Task<bool> MarkNotificationAsReadAsync(int notificationId);

    }
}
