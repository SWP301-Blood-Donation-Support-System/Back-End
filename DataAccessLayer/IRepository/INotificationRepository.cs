using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationByIdAsync(int notiId);
        Task<IEnumerable<Notification>> GetNotificationByTypeIdAsync(int notiTypeId);
        Task<IEnumerable<Notification>> GetAllNotificationAsync();
        Task<bool> SoftDeleteNotificationAsync(int notiId);
        Task<bool> UpdateNotificationAsync(Notification notification);
    }
}
