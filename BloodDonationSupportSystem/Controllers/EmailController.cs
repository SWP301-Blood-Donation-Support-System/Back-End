using BusinessLayer.IService;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public EmailController(IUserServices userServices)
        {
            _userServices = userServices ?? throw new ArgumentNullException(nameof(userServices));
        }

        /// <summary>
        /// Sends an email to a single recipient
        /// </summary>
        /// <param name="request">Email request with subject, body, and recipient</param>
        /// <returns>Status of the email sending operation</returns>
        [HttpPost("send")]
        [ProducesResponseType(typeof(EmailResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SendEmail([FromBody] EmailRequestDTO request)
        {
            try
            {
                // Validate model state is automatically done by ApiController attribute
                _userServices.SendMail(request.Subject, request.Body, request.Recipient);

                return Ok(new EmailResponseDTO
                {
                    Success = true,
                    Message = "Email sent successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EmailResponseDTO
                {
                    Success = false,
                    Message = $"Failed to send email: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Sends the same email to multiple recipients
        /// </summary>
        /// <param name="request">Email request with subject, body, and a list of recipients</param>
        /// <returns>Status of the bulk email sending operation</returns>
        [HttpPost("send-bulk")]
        [ProducesResponseType(typeof(EmailResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SendBulkEmail([FromBody] BulkEmailRequestDTO request)
        {
            var failedRecipients = new List<string>();

            try
            {
                foreach (var recipient in request.Recipients)
                {
                    try
                    {
                        _userServices.SendMail(request.Subject, request.Body, recipient);
                    }
                    catch
                    {
                        failedRecipients.Add(recipient);
                    }
                }

                if (failedRecipients.Count == 0)
                {
                    return Ok(new EmailResponseDTO
                    {
                        Success = true,
                        Message = "All emails sent successfully"
                    });
                }
                else if (failedRecipients.Count < request.Recipients.Count)
                {
                    return Ok(new EmailResponseDTO
                    {
                        Success = true,
                        Message = "Some emails failed to send",
                        FailedRecipients = failedRecipients
                    });
                }
                else
                {
                    return StatusCode(500, new EmailResponseDTO
                    {
                        Success = false,
                        Message = "All emails failed to send",
                        FailedRecipients = failedRecipients
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EmailResponseDTO
                {
                    Success = false,
                    Message = $"Failed to process bulk email request: {ex.Message}",
                    FailedRecipients = failedRecipients.Count == 0 ? request.Recipients : failedRecipients
                });
            }
        }

        /// <summary>
        /// Sends a template-based email with dynamic content
        /// </summary>
        /// <param name="request">Email request with placeholder values</param>
        /// <returns>Status of the email sending operation</returns>
        [HttpPost("send-template")]
        [ProducesResponseType(typeof(EmailResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SendTemplateEmail([FromBody] EmailRequestDTO request)
        {
            try
            {
                // This is a simple implementation - in a real-world scenario,
                // you might want to use a template engine like Razor or similar
                var templateBody = request.Body;
                
                // Process the template here (e.g., replace placeholders)
                // For now, we're just passing it through
                
                _userServices.SendMail(request.Subject, templateBody, request.Recipient);

                return Ok(new EmailResponseDTO
                {
                    Success = true,
                    Message = "Template email sent successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EmailResponseDTO
                {
                    Success = false,
                    Message = $"Failed to send template email: {ex.Message}"
                });
            }
        }
    }
}