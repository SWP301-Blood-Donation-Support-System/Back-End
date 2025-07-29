using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.DTO;
using System.Security.Claims;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// Store image URL - Frontend handles actual image upload to cloud services
        /// </summary>
        /// <param name="request">Request containing image URL</param>
        /// <returns>Confirmation of URL storage</returns>
        [HttpPost("store-url")]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult StoreImageUrl([FromBody] StoreImageUrlRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.ImageUrl))
                {
                    return BadRequest(new { status = "failed", message = "URL ?nh không ???c ?? tr?ng" });
                }

                // Validate URL format
                if (!Uri.TryCreate(request.ImageUrl, UriKind.Absolute, out Uri? uriResult) ||
                    (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    return BadRequest(new { status = "failed", message = "URL ?nh không h?p l?" });
                }

                return Ok(new 
                { 
                    status = "success", 
                    message = "L?u URL ?nh thành công",
                    data = new
                    {
                        imageUrl = request.ImageUrl,
                        storedAt = DateTime.UtcNow.AddHours(7);
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "L?i h? th?ng khi l?u URL ?nh", error = ex.Message });
            }
        }

        /// <summary>
        /// Store multiple image URLs
        /// </summary>
        /// <param name="request">Request containing multiple image URLs</param>
        /// <returns>Confirmation of URLs storage</returns>
        [HttpPost("store-multiple-urls")]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult StoreMultipleImageUrls([FromBody] StoreMultipleImageUrlsRequest request)
        {
            try
            {
                if (request?.ImageUrls == null || !request.ImageUrls.Any())
                {
                    return BadRequest(new { status = "failed", message = "Danh sách URL ?nh không ???c ?? tr?ng" });
                }

                if (request.ImageUrls.Count > 10)
                {
                    return BadRequest(new { status = "failed", message = "Ch? ???c phép l?u t?i ?a 10 URL ?nh cùng lúc" });
                }

                var validUrls = new List<string>();
                var invalidUrls = new List<string>();

                foreach (var url in request.ImageUrls)
                {
                    if (!string.IsNullOrEmpty(url) && 
                        Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
                        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        validUrls.Add(url);
                    }
                    else
                    {
                        invalidUrls.Add(url);
                    }
                }

                return Ok(new 
                { 
                    status = "success", 
                    message = $"L?u thành công {validUrls.Count}/{request.ImageUrls.Count} URL ?nh",
                    data = new
                    {
                        totalUrls = request.ImageUrls.Count,
                        validUrls = validUrls.Count,
                        invalidUrls = invalidUrls.Count,
                        imageUrls = validUrls,
                        failedUrls = invalidUrls,
                        storedAt = DateTime.UtcNow.AddHours(7);
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "L?i h? th?ng khi l?u URL ?nh", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate if an image URL is accessible
        /// </summary>
        /// <param name="request">Request containing image URL to validate</param>
        /// <returns>Validation result</returns>
        [HttpPost("validate-url")]
        public async Task<IActionResult> ValidateImageUrl([FromBody] ValidateImageUrlRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.ImageUrl))
                {
                    return BadRequest(new { status = "failed", message = "URL ?nh không ???c ?? tr?ng" });
                }

                // Validate URL format
                if (!Uri.TryCreate(request.ImageUrl, UriKind.Absolute, out Uri? uriResult) ||
                    (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    return BadRequest(new { status = "failed", message = "URL ?nh không h?p l?", isValid = false });
                }

                // Optional: Check if URL is accessible (with timeout)
                var isAccessible = await CheckUrlAccessibility(request.ImageUrl);

                return Ok(new 
                { 
                    status = "success", 
                    data = new
                    {
                        imageUrl = request.ImageUrl,
                        isValid = true,
                        isAccessible = isAccessible,
                        checkedAt = DateTime.UtcNow.AddHours(7);
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "L?i h? th?ng khi ki?m tra URL ?nh", error = ex.Message });
            }
        }

        private async Task<bool> CheckUrlAccessibility(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(5); // 5 second timeout
                
                var response = await httpClient.GetAsync(url);
                return response.IsSuccessStatusCode && 
                       response.Content.Headers.ContentType?.MediaType?.StartsWith("image/") == true;
            }
            catch
            {
                return false;
            }
        }
    }
}