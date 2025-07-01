using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface INotificationService
    {
        Task AddAsync(NotificationDTO notification);
        Task<IEnumerable<Notification>> GetNotificationByIdAsync(int notiId);
        Task<IEnumerable<Notification>> GetNotificationByTypeIdAsync(int notiTypeId);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task<bool> SoftDeleteNotificationAsync(int notiId);
        Task<bool> UpdateNotificationAsync(NotificationDTO notification);
    }
}
