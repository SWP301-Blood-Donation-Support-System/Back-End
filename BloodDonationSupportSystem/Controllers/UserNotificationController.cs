using BusinessLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/user-notifications")]
[ApiController]
[Authorize] // Yêu cầu người dùng phải đăng nhập
public class UserNotificationController : ControllerBase
{
    private readonly IUserNotificationService _userNotificationService;

    public UserNotificationController(IUserNotificationService userNotificationService)
    {
        _userNotificationService = userNotificationService;
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetMyUnreadNotifications()
    {
        // Lấy ID của người dùng đang đăng nhập từ token
        var userIdString = User.FindFirst("UserID")?.Value;
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var notifications = await _userNotificationService.GetUnreadNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpPost("{id}/mark-as-read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await _userNotificationService.MarkNotificationAsReadAsync(id);
        if (!result)
        {
            return NotFound("Notification not found or already read.");
        }
        return Ok("Notification marked as read.");
    }
}