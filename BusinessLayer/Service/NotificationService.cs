using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper Mapper;
        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            Mapper = mapper;
        }
        public async Task AddAsync(NotificationDTO notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification), "Notification cannot be null");
            }
            var entity = Mapper.Map<Notification>(notification);
            var result = await _notificationRepository.AddAsync(entity);
            await _notificationRepository.SaveChangesAsync();
        }
        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _notificationRepository.GetAllAsync();
        }
        public async Task<IEnumerable<Notification>> GetNotificationByIdAsync(int notiId)
        {
            if (notiId <= 0)
                throw new ArgumentOutOfRangeException(nameof(notiId), "ID must be greater than zero");
            return await _notificationRepository.GetNotificationByIdAsync(notiId);
        }
        public async Task<IEnumerable<Notification>> GetNotificationByTypeIdAsync(int notiTypeId)
        {
            if (notiTypeId <= 0)
                throw new ArgumentOutOfRangeException(nameof(notiTypeId), "ID must be greater than zero");
            return await _notificationRepository.GetNotificationByTypeIdAsync(notiTypeId);
        }
        public async Task<bool> SoftDeleteNotificationAsync(int notiId)
        {
            if (notiId <= 0)
                throw new ArgumentOutOfRangeException(nameof(notiId), "ID must be greater than zero");
            return await _notificationRepository.SoftDeleteNotificationAsync(notiId);
        }
        public async Task<bool> UpdateNotificationAsync(int notificationId, NotificationDTO notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification), "Notification cannot be null");
            }

            if (notificationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(notificationId), "Notification ID must be greater than zero");
            }

            // Lấy notification hiện tại từ database
            var existingNotification = await _notificationRepository.GetByIdAsync(notificationId);
            if (existingNotification == null)
            {
                return false; // Không tìm thấy notification
            }

            // Cập nhật các thuộc tính từ DTO
            existingNotification.NotificationTypeId = notification.NotificationTypeId;
            existingNotification.Subject = notification.Subject;
            existingNotification.Message = notification.Message;
            existingNotification.UpdatedAt = DateTime.UtcNow.AddHours(7);;

            await _notificationRepository.UpdateAsync(existingNotification);
            await _notificationRepository.SaveChangesAsync();
            return true;
        }
    }
}