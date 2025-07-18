using BusinessLayer.IService;
using DataAccessLayer.IRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using BloodDonationSupportSystem.Hubs; 
namespace BusinessLayer.QuartzJobs.Job
{
    [DisallowConcurrentExecution]
    public class DonationReminderJob : IJob
    {
        private readonly ILogger<DonationReminderJob> _logger;
        private readonly IUserRepository _userRepository;
        // Inject IHubContext<NotificationHub> mà không có interface
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserNotificationService _userNotificationService;

        public DonationReminderJob(
            ILogger<DonationReminderJob> logger,
            IUserRepository userRepository,
            IHubContext<NotificationHub> hubContext, // Cập nhật kiểu dữ liệu ở đây
            IUserNotificationService userNotificationService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _userNotificationService = userNotificationService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("--- Running Donation Reminder Job ---");
            try
            {
                var targetDate = DateTime.UtcNow.AddDays(7).Date;
                var usersToNotify = await _userRepository.GetEligibleDonorsAsync();

                foreach (var user in usersToNotify)
                {
                    if (user.NextEligibleDonationDate.HasValue && user.NextEligibleDonationDate.Value.Date == targetDate)
                    {
                        var subject = "Nhắc nhở lịch hiến máu";
                        var message = $"Xin chào {user.FullName}, bạn sẽ có thể hiến máu trở lại vào ngày {user.NextEligibleDonationDate.Value.ToString("dd/MM/yyyy")}. Hãy sẵn sàng để cứu người nhé!";

                        // 1. Lưu thông báo vào DB
                        await _userNotificationService.CreateNotificationForUserAsync(user.UserId, subject, message, 1);

                        // 2. Gửi thông báo real-time bằng cách dùng tên phương thức dạng chuỗi
                        await _hubContext.Clients.Group(user.UserId.ToString())
                            .SendAsync("ReceiveNotification", subject, message); // <-- THAY ĐỔI Ở ĐÂY

                        _logger.LogInformation($"Sent reminder notification to user ID: {user.UserId}, Email: {user.Email}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the Donation Reminder Job.");
            }
            _logger.LogInformation("--- Donation Reminder Job Finished ---");
        }
    }
}