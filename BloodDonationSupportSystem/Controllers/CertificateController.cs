using BusinessLayer.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;

        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService ?? 
                throw new ArgumentNullException(nameof(certificateService));
        }

        [HttpGet("generate/{registrationId}")]
        public async Task<IActionResult> GenerateCertificate(int registrationId)
        {
            try
            {
                var pdfBytes = await _certificateService.GenerateCertificateAsync(registrationId);
                return File(
                    pdfBytes, 
                    "application/pdf", 
                    $"giay-chung-nhan-hien-mau.pdf");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle any nested exceptions
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new
                {
                    message = $"An error occurred: {ex.Message}",
                    innerException = innerMessage
                });
            }
        }

    }
}