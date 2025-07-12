using DataAccessLayer.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    /// <summary>
    /// Repository interface for UserNotification entity.
    /// Inherits generic methods and defines specific methods for user notifications.
    /// </summary>
    public interface IUserNotificationRepository : IGenericRepository<UserNotification>
    {
        /// <summary>
        /// Gets all notifications for a specific recipient.
        /// </summary>
        /// <param name="recipientId">The ID of the recipient user.</param>
        /// <returns>A collection of UserNotification objects.</returns>
        Task<IEnumerable<UserNotification>> GetNotificationsByRecipientIdAsync(int recipientId);

        /// <summary>
        /// Gets all unread notifications for a specific recipient.
        /// </summary>
        /// <param name="recipientId">The ID of the recipient user.</param>
        /// <returns>A collection of unread UserNotification objects.</returns>
        Task<IEnumerable<UserNotification>> GetUnreadNotificationsByRecipientIdAsync(int recipientId);

        /// <summary>
        /// Soft deletes a user notification.
        /// </summary>
        /// <param name="userNotificationId">The ID of the user notification to delete.</param>
        /// <returns>True if deletion was successful, otherwise false.</returns>
        Task<bool> SoftDeleteUserNotificationAsync(int userNotificationId);
    }
}