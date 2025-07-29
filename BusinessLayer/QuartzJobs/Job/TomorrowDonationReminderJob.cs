using BusinessLayer.IService;
using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.QuartzJobs.Job
{
    [DisallowConcurrentExecution] // ??m b?o job không ch?y ch?ng chéo n?u l?n ch?y tr??c ch?a xong
    public class TomorrowDonationReminderJob : IJob
    {
        private readonly ILogger<TomorrowDonationReminderJob> _logger;
        private readonly IUserServices _userServices;

        public TomorrowDonationReminderJob(ILogger<TomorrowDonationReminderJob> logger, IUserServices userServices)
        {
            _logger = logger;
            _userServices = userServices;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("=== Starting Tomorrow Donation Reminder Job ===");
            var startTime = DateTime.UtcNow;

            try
            {
                var result = await _userServices.SendTomorrowDonationRemindersAsync();

                _logger.LogInformation($"Tomorrow Donation Reminder Job completed successfully:");
                _logger.LogInformation($"- Total upcoming donations: {result.TotalUpcomingDonations}");
                _logger.LogInformation($"- Successful notifications: {result.SuccessfulNotifications}");
                _logger.LogInformation($"- Failed notifications: {result.FailedNotifications}");
                _logger.LogInformation($"- Execution time: {result.ExecutionTime.TotalSeconds:F2} seconds");

                if (result.ErrorMessages.Count > 0)
                {
                    _logger.LogWarning($"Errors occurred during job execution:");
                    foreach (var error in result.ErrorMessages)
                    {
                        _logger.LogWarning($"- {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                var executionTime = DateTime.UtcNow - startTime;
                _logger.LogError(ex, $"Tomorrow Donation Reminder Job failed after {executionTime.TotalSeconds:F2} seconds");
                _logger.LogError($"Error message: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }

            _logger.LogInformation("=== Tomorrow Donation Reminder Job Finished ===");
        }
    }
}