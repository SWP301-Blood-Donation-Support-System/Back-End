using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    /// <summary>
    /// API Controller quản lý thông báo hệ thống
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Khởi tạo NotificationController
        /// </summary>
        /// <param name="notificationService">Service xử lý logic thông báo</param>
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy danh sách tất cả thông báo trong hệ thống
        /// </summary>
        /// <returns>Danh sách thông báo</returns>
        /// <response code="200">Trả về danh sách thông báo thành công</response>
        /// <response code="500">Lỗi server nội bộ</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllNotification()
        {
            try
            {
                var records = await _notificationService.GetAllNotificationsAsync();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error retrieving notifications", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông báo theo ID
        /// </summary>
        /// <param name="notiId">ID của thông báo cần lấy</param>
        /// <returns>Thông tin chi tiết thông báo</returns>
        /// <response code="200">Trả về thông tin thông báo thành công</response>
        /// <response code="400">ID không hợp lệ (phải lớn hơn 0)</response>
        /// <response code="404">Không tìm thấy thông báo với ID đã cho</response>
        /// <response code="500">Lỗi server nội bộ</response>
        [HttpGet("{notiId:int}")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotificationById(int notiId)
        {
            try
            {
                if (notiId <= 0)
                    return BadRequest(new { message = "ID must be greater than zero" });

                var notification = await _notificationService.GetNotificationByIdAsync(notiId);
                if (notification == null || !notification.Any())
                    return NotFound(new { message = $"No notification found with ID {notiId}" });

                return Ok(notification);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = "Invalid notification ID", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = $"Error retrieving notification with ID {notiId}", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách thông báo theo loại thông báo
        /// </summary>
        /// <param name="notiTypeId">ID loại thông báo (1: Thông báo chung, 2: Nhắc nhở hiến máu, 3: Khẩn cấp, v.v.)</param>
        /// <returns>Danh sách thông báo theo loại</returns>
        /// <response code="200">Trả về danh sách thông báo theo loại thành công</response>
        /// <response code="400">ID loại thông báo không hợp lệ</response>
        /// <response code="404">Không tìm thấy thông báo nào với loại đã cho</response>
        /// <response code="500">Lỗi server nội bộ</response>
        [HttpGet("type/{notiTypeId:int}")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotificationByTypeId(int notiTypeId)
        {
            try
            {
                if (notiTypeId <= 0)
                    return BadRequest(new { message = "Notification type ID must be greater than zero" });

                var notifications = await _notificationService.GetNotificationByTypeIdAsync(notiTypeId);
                if (notifications == null || !notifications.Any())
                    return NotFound(new { message = $"No notifications found for type ID {notiTypeId}" });

                return Ok(notifications);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = "Invalid notification type ID", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = $"Error retrieving notifications for type ID {notiTypeId}", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo thông báo mới
        /// </summary>
        /// <param name="notification">Thông tin thông báo cần tạo</param>
        /// <returns>Kết quả tạo thông báo</returns>
        /// <response code="200">Tạo thông báo thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi tạo thông báo</response>
        /// <remarks>
        /// Ví dụ request body:
        /// 
        ///     POST /api/notification
        ///     {
        ///         "notificationTypeId": 1,
        ///         "subject": "Thông báo quan trọng",
        ///         "message": "Nội dung thông báo chi tiết..."
        ///     }
        /// 
        /// Các loại thông báo phổ biến:
        /// - 1: Thông báo chung
        /// - 2: Nhắc nhở hiến máu  
        /// - 3: Thông báo khẩn cấp
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddNotification([FromBody] NotificationDTO notification)
        {
            try
            {
                if (notification == null)
                    return BadRequest(new { message = "Notification data cannot be null" });

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid model state", errors = ModelState });

                await _notificationService.AddAsync(notification);
                return Ok(new { message = "Notification added successfully" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = "Missing required notification data", error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = "Invalid operation", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error adding notification", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa mềm thông báo (đánh dấu IsDeleted = true)
        /// </summary>
        /// <param name="notiId">ID của thông báo cần xóa</param>
        /// <returns>Kết quả xóa thông báo</returns>
        /// <response code="200">Xóa thông báo thành công</response>
        /// <response code="400">ID không hợp lệ</response>
        /// <response code="404">Không tìm thấy thông báo để xóa</response>
        /// <response code="500">Lỗi server nội bộ</response>
        /// <remarks>
        /// Thao tác này chỉ đánh dấu thông báo là đã xóa (soft delete), 
        /// không xóa vĩnh viễn khỏi database để đảm bảo tính toàn vẹn dữ liệu.
        /// </remarks>
        [HttpPatch("{notiId:int}/soft-delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SoftDeleteNotification(int notiId)
        {
            try
            {
                if (notiId <= 0)
                    return BadRequest(new { message = "Notification ID must be greater than zero" });

                var result = await _notificationService.SoftDeleteNotificationAsync(notiId);
                if (!result)
                    return NotFound(new { message = $"Notification with ID {notiId} not found or already deleted" });

                return Ok(new { message = "Notification deleted successfully" });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = "Invalid notification ID", error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = "Cannot delete notification", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = $"Error deleting notification with ID {notiId}", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin thông báo
        /// </summary>
        /// <param name="notificationId">ID của thông báo cần cập nhật</param>
        /// <param name="notification">Thông tin thông báo mới</param>
        /// <returns>Kết quả cập nhật thông báo</returns>
        /// <response code="200">Cập nhật thông báo thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="404">Không tìm thấy thông báo để cập nhật</response>
        /// <response code="500">Lỗi server khi cập nhật</response>
        /// <remarks>
        /// Ví dụ request:
        /// 
        ///     PUT /api/notification/123
        ///     {
        ///         "notificationTypeId": 2,
        ///         "subject": "Thông báo đã cập nhật",
        ///         "message": "Nội dung mới của thông báo..."
        ///     }
        /// 
        /// Lưu ý:
        /// - NotificationId trong URL sẽ được sử dụng để xác định thông báo cần cập nhật
        /// - Trường NotificationId trong body (nếu có) sẽ được bỏ qua
        /// - Hệ thống sẽ tự động cập nhật UpdatedAt timestamp
        /// </remarks>
        [HttpPut("{notificationId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateNotification(int notificationId, [FromBody] NotificationDTO notification)
        {
            try
            {
                if (notification == null)
                    return BadRequest(new { message = "Notification data cannot be null" });

                if (notificationId <= 0)
                    return BadRequest(new { message = "Notification ID must be greater than zero" });

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid model state", errors = ModelState });

                var result = await _notificationService.UpdateNotificationAsync(notificationId, notification);
                if (!result)
                    return NotFound(new { message = $"Notification with ID {notificationId} not found" });

                return Ok(new { message = "Notification updated successfully" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = "Missing required notification data", error = ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = "Invalid notification ID", error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = "Notification not found", error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = "Invalid operation", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = $"Error updating notification with ID {notificationId}", error = ex.Message });
            }
        }
    }
}
