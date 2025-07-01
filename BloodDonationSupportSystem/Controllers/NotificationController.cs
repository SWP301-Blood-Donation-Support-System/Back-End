using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;


        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNotification()
        {
            var records = await _notificationService.GetAllNotificationsAsync();
            return Ok(records);
        }
        [HttpGet("{notiId:int}")]
        public async Task<IActionResult> GetNotificationById(int notiId)
        {
            if (notiId <= 0)
                return BadRequest("ID must be greater than zero");
            var notification = await _notificationService.GetNotificationByIdAsync(notiId);
            if (notification == null || !notification.Any())
                return NotFound();
            return Ok(notification);
        }
        [HttpGet("type/{notiTypeId:int}")]
        public async Task<IActionResult> GetNotificationByTypeId(int notiTypeId)
        {
            if (notiTypeId <= 0)
                return BadRequest("ID must be greater than zero");
            var notifications = await _notificationService.GetNotificationByTypeIdAsync(notiTypeId);
            if (notifications == null || !notifications.Any())
                return NotFound();
            return Ok(notifications);
        }
        [HttpPost]
        public async Task<IActionResult> AddNotification([FromBody] NotificationDTO notification)
        {
            if (notification == null)
                return BadRequest("Notification cannot be null");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _notificationService.AddAsync(notification);
                return Ok(new { message = "Notification added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding notification: {ex.Message}");
            }
        }
        [HttpDelete("{notiId:int}")]
        public async Task<IActionResult> SoftDeleteNotification(int notiId)
        {
            if (notiId <= 0)
                return BadRequest("ID must be greater than zero");
            var result = await _notificationService.SoftDeleteNotificationAsync(notiId);
            if (!result)
                return NotFound();
            return Ok(new { message = "Notification deleted successfully." });
        }
        [HttpPut("notification")]
        public async Task<IActionResult> UpdateNotification([FromBody] NotificationDTO notification)
        {
            if (notification == null)
                return BadRequest("Notification cannot be null");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _notificationService.UpdateNotificationAsync(notification);
                if (!result)
                    return NotFound();
                return Ok(new { message = "Notification updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating notification: {ex.Message}");
            }
        }
    }
}
