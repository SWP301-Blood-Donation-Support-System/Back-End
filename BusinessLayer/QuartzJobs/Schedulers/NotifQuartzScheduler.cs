using BuisinessLayer.Utils.EmailConfiguration;
using BusinessLayer.QuartzJobs.Job;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.QuartzJobs.Schedulers
{
    public class NotifQuartzScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<NotifQuartzScheduler> _logger;

        public NotifQuartzScheduler(ISchedulerFactory schedulerFactory, ILogger<NotifQuartzScheduler> logger)
        {
            _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Schedules a notification job to send an email
        /// </summary>
        /// <param name="notifyId">Unique identifier for the notification</param>
        /// <param name="message">Message object containing email details</param>
        /// <returns>True if the job was scheduled successfully</returns>
        public async Task<bool> ScheduleNotifyJob(string notifyID, Message message)
        {
            if (message == null)
            {
                _logger.LogError("Cannot schedule notification: Message is null");
                return false;
            }

            // Get scheduler
            IScheduler scheduler = await _schedulerFactory.GetScheduler();

            // Convert Message.To (List<MailboxAddress>) to comma-separated string of email addresses
            string recipientsString = string.Join(",", message.To.Select(t => t.Address));

            // Create job with necessary data for NotifyJobs.Execute
            var job = JobBuilder.Create<NotifyJobs>()
                .WithIdentity($"notifyJob_{notifyID}", "notificationGroup")
                .UsingJobData("Subject", message.Subject)
                .UsingJobData("Body", message.Content)
                .UsingJobData("Recipients", recipientsString)
                .Build();

            // Create trigger to run the job immediately
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"notifyTrigger_{notifyID}", "notificationGroup")
                .StartAt(DateBuilder.FutureDate(1,IntervalUnit.Minute))
                .WithSimpleSchedule(action => action.WithIntervalInMinutes(1).RepeatForever())
                .StartNow()
                .Build();

            // Schedule the job
            await scheduler.ScheduleJob(job, trigger);

            _logger.LogInformation($"Notification job scheduled with ID: {notifyID}, " +
                $"Subject: {message.Subject}, Recipients: {recipientsString.Substring(0, Math.Min(50, recipientsString.Length))}...");

            return true;
        }

        /// <summary>
        /// Schedules a notification job to be sent at a specific time
        /// </summary>
        /// <param name="notifyId">Unique identifier for the notification</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content</param>
        /// <param name="recipients">List of email recipients</param>
        /// <param name="startAt">When to send the notification (defaults to immediately)</param>
        /// <returns>True if the job was scheduled successfully</returns>
        
    }
}
