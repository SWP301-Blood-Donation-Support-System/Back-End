using BuisinessLayer.Utils.EmailConfiguration;
using BusinessLayer.IService;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.QuartzJobs.Job
{
    public class NotifyJobs : IJob
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<NotifyJobs> _logger;    
        
        public NotifyJobs(IEmailService emailService, ILogger<NotifyJobs> logger)
        {
            this._emailService = emailService;
            this._logger = logger;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var subject = context.JobDetail.JobDataMap.GetString("Subject");
                var body = context.JobDetail.JobDataMap.GetString("Body");
                var recipientsString = context.JobDetail.JobDataMap.GetString("Recipients");
                
                if (string.IsNullOrWhiteSpace(subject))
                {
                    _logger.LogError("Subject is missing in the job data");
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(body))
                {
                    _logger.LogError("Body is missing in the job data");
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(recipientsString))
                {
                    _logger.LogError("Recipients string is missing in the job data");
                    return;
                }
                
                // Parse the recipients string into a List<string>
                // Assuming recipients are comma-separated or semicolon-separated
                var recipientsList = recipientsString.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(email => email.Trim())
                    .Where(email => !string.IsNullOrWhiteSpace(email))
                    .ToList();
                
                if (recipientsList.Count == 0)
                {
                    _logger.LogWarning("No valid recipients found in the job data");
                    return;
                }

                // Create a Message object with the required parameters
                var message = new Message(
                    to: recipientsList,
                    subject: subject,
                    content: body  // Note: The Message class uses Content, not Body
                );
                
                _emailService.SendEmail(message);
                _logger.LogInformation($"Email notification sent to {recipientsList.Count} recipients");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing email notification job");
                // We're not rethrowing here to prevent the job from failing completely
            }
        }
    }
}
