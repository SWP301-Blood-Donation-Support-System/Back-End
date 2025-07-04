using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IUserNotificationRepository : IGenericRepository<UserNotification>
    {
        Task <UserNotification> AddAsync(UserNotification userNotification);
        Task<UserNotification> UpdateAsync(UserNotification userNotification);
        Task<bool> SoftDeleteNotificationAsync(int unotiId);
        Task<IEnumerable<UserNotification>> GetUserNotificationByNotificationIdAsync(int notiId);
        Task<IEnumerable<UserNotification>> GetUserNotificationByRecipientIdAsync(int repiId);
        Task<IEnumerable<UserNotification>> GetAllAsync();
    }
}
