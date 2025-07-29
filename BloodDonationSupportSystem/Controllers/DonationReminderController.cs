using BusinessLayer.IService;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace BloodDonationSupportSystem.Controllers
{
    /// <summary>
    /// API Controller quản lý tính năng nhắc nhở hiến máu
    /// </summary>
    [Route("api/[controller]")]
    [ApiController] 
    public class DonationReminderController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IUserNotificationService _userNotificationService;

        /// <summary>
        /// Khởi tạo DonationReminderController
        /// </summary>
        public DonationReminderController(IUserServices userServices, IUserNotificationService userNotificationService)
        {
            _userServices = userServices;
            _userNotificationService = userNotificationService;
        }

        /// <summary>
        /// Lấy danh sách người có thể hiến máu trong 3 ngày tới
        /// </summary>
        /// <param name="daysAhead">Số ngày tới (mặc định 3 ngày)</param>
        /// <returns>Danh sách người có thể hiến máu</returns>
        /// <response code="200">Trả về danh sách thành công</response>
        /// <response code="400">Tham số không hợp lệ</response>
        /// <response code="500">Lỗi server nội bộ</response>
        /// <remarks>
        /// Endpoint công khai, ai cũng có thể truy cập.
        /// 
        /// Ví dụ request:
        ///     GET /api/donationreminder/upcoming-eligible?daysAhead=3
        ///     
        /// Response sẽ bao gồm:
        /// - UserId, FullName, Email, PhoneNumber
        /// - BloodTypeName, NextEligibleDonationDate
        /// - DaysUntilEligible, LastDonationDate, IsActive
        /// </remarks>
        [HttpGet("upcoming-eligible")]
        [ProducesResponseType(typeof(IEnumerable<UpcomingEligibleDonorsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUpcomingEligibleDonors([FromQuery] int daysAhead = 3)
        {
            try
            {
                if (daysAhead < 1 || daysAhead > 30)
                {
                    return BadRequest(new { message = "Số ngày phải từ 1 đến 30" });
                }

                var upcomingDonors = await _userServices.GetUpcomingEligibleDonorsAsync(daysAhead);
                
                return Ok(new { 
                    message = $"Danh sách người có thể hiến máu trong {daysAhead} ngày tới",
                    daysAhead = daysAhead,
                    totalCount = upcomingDonors.Count(),
                    data = upcomingDonors,
                    retrievedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = "Tham số không hợp lệ", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Lỗi khi lấy danh sách người có thể hiến máu", error = ex.Message });
            }
        }

        /// <summary>
        /// Gửi thông báo nhắc nhở hàng loạt cho danh sách người hiến máu
        /// </summary>
        /// <param name="request">Thông tin request gửi thông báo</param>
        /// <returns>Kết quả gửi thông báo hàng loạt</returns>
        /// <response code="200">Gửi thông báo thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="401">Không thể xác định thông tin người dùng</response>
        /// <response code="500">Lỗi server khi gửi thông báo</response>
        /// <remarks>
        /// Endpoint công khai, ai cũng có thể sử dụng.
        /// 
        /// Ví dụ request body:
        /// 
        ///     POST /api/donationreminder/send-bulk-reminders
        ///     {
        ///         "userIds": [1, 2, 3, 4, 5],
        ///         "customMessage": "Xin chào {UserName}, bạn đã có thể hiến máu trở lại! Hãy đăng ký ngay hôm nay.",
        ///         "sendEmail": true,
        ///         "sendNotification": true
        ///     }
        /// 
        /// Custom message có thể sử dụng placeholders:
        /// - {UserName}: Tên người nhận
        /// - {AdminName}: Tên người gửi
        /// 
        /// Response bao gồm:
        /// - TotalTargetUsers: Tổng số người được chọn
        /// - SuccessfulNotifications: Số thông báo gửi thành công
        /// - FailedNotifications: Số thông báo gửi thất bại
        /// - ErrorMessages: Chi tiết lỗi (nếu có)
        /// </remarks>
        [HttpPost("send-bulk-reminders")]
        [ProducesResponseType(typeof(BulkReminderResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendBulkReminders([FromBody] BulkReminderRequestDTO request)
        {
            try
            {
                // Lấy ID của người dùng hiện tại (nếu có đăng nhập)
                var userIdClaim = User.FindFirst("UserID")?.Value;
                var currentUserId = int.TryParse(userIdClaim, out var userId) ? userId : 0;

                // Nếu không có người dùng đăng nhập, sử dụng ID mặc định
                if (currentUserId == 0)
                {
                    currentUserId = 1; // Sử dụng admin mặc định hoặc system user
                }

                // Validation
                if (request == null)
                {
                    return BadRequest(new { message = "Dữ liệu request không được để trống" });
                }

                if (request.UserIds == null || !request.UserIds.Any())
                {
                    return BadRequest(new { message = "Danh sách UserIds không được để trống" });
                }

                if (request.UserIds.Count > 100)
                {
                    return BadRequest(new { message = "Không thể gửi thông báo cho quá 100 người trong một lần" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
                }

                // Gửi thông báo hàng loạt
                var result = await _userServices.SendBulkDonationRemindersAsync(request, currentUserId);

                return Ok(new { 
                    message = "Đã hoàn thành gửi thông báo hàng loạt",
                    result = result
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = "Thiếu dữ liệu bắt buộc", error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = "Thao tác không hợp lệ", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Lỗi khi gửi thông báo hàng loạt", error = ex.Message });
            }
        }

        /// <summary>
        /// Gửi thông báo nhắc nhở cho một người cụ thể
        /// </summary>
        /// <param name="userId">ID của người cần gửi thông báo</param>
        /// <param name="customMessage">Nội dung tùy chỉnh (tùy chọn)</param>
        /// <returns>Kết quả gửi thông báo</returns>
        /// <response code="200">Gửi thông báo thành công</response>
        /// <response code="400">User ID không hợp lệ</response>
        /// <response code="404">Không tìm thấy user</response>
        /// <response code="500">Lỗi server</response>
        /// <remarks>
        /// Endpoint công khai, ai cũng có thể sử dụng.
        /// 
        /// Ví dụ request:
        ///     POST /api/donationreminder/send-reminder/123
        ///     Content-Type: application/json
        ///     
        ///     "Tin nhắn tùy chỉnh cho người này"
        /// </remarks>
        [HttpPost("send-reminder/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendSingleReminder(int userId, [FromBody] string? customMessage = null)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "User ID phải lớn hơn 0" });
                }

                // Lấy ID của người dùng hiện tại (nếu có đăng nhập)
                var userIdClaim = User.FindFirst("UserID")?.Value;
                var currentUserId = int.TryParse(userIdClaim, out var currentUser) ? currentUser : 1;

                // Tạo request cho single user
                var request = new BulkReminderRequestDTO
                {
                    UserIds = new List<int> { userId },
                    CustomMessage = customMessage,
                    SendEmail = true,
                    SendNotification = true
                };

                var result = await _userServices.SendBulkDonationRemindersAsync(request, currentUserId);

                if (result.SuccessfulNotifications > 0)
                {
                    return Ok(new { 
                        message = "Gửi thông báo thành công",
                        userId = userId,
                        result = result
                    });
                }
                else
                {
                    return NotFound(new { 
                        message = "Không thể gửi thông báo",
                        userId = userId,
                        errors = result.ErrorMessages
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = $"Lỗi khi gửi thông báo cho user {userId}", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách người có lịch hiến máu vào ngày mai
        /// </summary>
        /// <returns>Danh sách người có lịch hiến vào ngày mai</returns>
        /// <response code="200">Trả về danh sách thành công</response>
        /// <response code="500">Lỗi server nội bộ</response>
        /// <remarks>
        /// Endpoint công khai để xem danh sách người có lịch hiến vào ngày mai.
        /// 
        /// Ví dụ request:
        ///     GET /api/donationreminder/tomorrow-schedules
        ///     
        /// Response sẽ bao gồm:
        /// - RegistrationId, DonorId, DonorName, DonorEmail
        /// - ScheduleDate, TimeSlotName, StartTime, EndTime
        /// - Location, BloodTypeName, StatusName
        /// </remarks>
        [HttpGet("tomorrow-schedules")]
        [ProducesResponseType(typeof(IEnumerable<TomorrowDonationScheduleDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTomorrowDonationSchedules()
        {
            try
            {
                var tomorrowSchedules = await _userServices.GetTomorrowDonationSchedulesAsync();
                var schedulesList = tomorrowSchedules.ToList();
                
                return Ok(new { 
                    message = "Danh sách người có lịch hiến máu vào ngày mai",
                    totalCount = schedulesList.Count,
                    data = schedulesList,
                    retrievedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Lỗi khi lấy danh sách lịch hiến máu ngày mai", error = ex.Message });
            }
        }

        /// <summary>
        /// Gửi thông báo nhắc nhở tự động cho những người có lịch hiến vào ngày mai
        /// </summary>
        /// <returns>Kết quả gửi thông báo tự động</returns>
        /// <response code="200">Gửi thông báo thành công</response>
        /// <response code="500">Lỗi server khi gửi thông báo</response>
        /// <remarks>
        /// Endpoint để trigger job gửi thông báo cho những người có lịch hiến vào ngày mai.
        /// Thường được gọi bởi scheduled job hoặc manual trigger.
        /// 
        /// Ví dụ request:
        ///     POST /api/donationreminder/send-tomorrow-reminders
        ///     
        /// Response bao gồm:
        /// - TotalUpcomingDonations: Tổng số lịch hiến vào ngày mai
        /// - SuccessfulNotifications: Số thông báo gửi thành công
        /// - FailedNotifications: Số thông báo gửi thất bại
        /// - ExecutionTime: Thời gian thực thi
        /// - ErrorMessages: Chi tiết lỗi (nếu có)
        /// </remarks>
        [HttpPost("send-tomorrow-reminders")]
        [ProducesResponseType(typeof(AutoReminderJobResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendTomorrowDonationReminders()
        {
            try
            {
                var result = await _userServices.SendTomorrowDonationRemindersAsync();

                return Ok(new { 
                    message = "Đã hoàn thành gửi thông báo nhắc nhở cho lịch hiến ngày mai",
                    result = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Lỗi khi gửi thông báo nhắc nhở tự động", error = ex.Message });
            }
        }
    }
}