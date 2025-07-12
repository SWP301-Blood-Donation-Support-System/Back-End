// In BusinessLayer/QuartzJobs/Job/AutoScheduleCreationJob.cs

using BusinessLayer.IService;
using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.QuartzJobs.Job
{
    [DisallowConcurrentExecution] // Đảm bảo job không chạy chồng chéo nếu lần chạy trước chưa xong
    public class AutoScheduleCreationJob : IJob
    {
        private readonly ILogger<AutoScheduleCreationJob> _logger;
        private readonly IDonationScheduleService _scheduleService;

        public AutoScheduleCreationJob(ILogger<AutoScheduleCreationJob> logger, IDonationScheduleService scheduleService)
        {
            _logger = logger;
            _scheduleService = scheduleService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("--- Running Auto Schedule Creation Job ---");

            try
            {
                await _scheduleService.CheckAndCreateSchedulesIfNeededAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the Auto Schedule Creation Job.");
            }

            _logger.LogInformation("--- Auto Schedule Creation Job Finished ---");
        }
    }
}