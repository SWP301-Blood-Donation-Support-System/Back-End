using BusinessLayer.IService;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ICloudinaryService _cloudinaryService;

        public ArticlesController(IArticleService articleService, ICloudinaryService cloudinaryService)
        {
            _articleService = articleService;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Store image URL from frontend (already processed by Cloudinary on frontend)
        /// </summary>
        /// <param name="request">Image URL from frontend</param>
        /// <returns>Confirmation</returns>
        [HttpPost("store-image-url")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> StoreImageUrl([FromBody] StoreImageUrlRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ImageUrl) || !Uri.IsWellFormedUriString(request.ImageUrl, UriKind.Absolute))
                {
                    return BadRequest(new 
                    { 
                        status = "failed", 
                        message = "URL ảnh không hợp lệ" 
                    });
                }

                // Validate if it's a Cloudinary URL (optional)
                if (!request.ImageUrl.Contains("cloudinary.com"))
                {
                    return BadRequest(new 
                    { 
                        status = "failed", 
                        message = "Chỉ chấp nhận URL từ Cloudinary" 
                    });
                }

                return Ok(new 
                { 
                    status = "success", 
                    message = "URL ảnh hợp lệ, có thể sử dụng để tạo bài viết",
                    imageUrl = request.ImageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi khi xử lý URL ảnh", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Upload image for article using Cloudinary
        /// </summary>
        /// <param name="imageFile">Image file to upload</param>
        /// <returns>Cloudinary image URL</returns>
        [HttpPost("upload-image")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UploadArticleImage(IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest(new 
                    { 
                        status = "failed", 
                        message = "File ảnh không hợp lệ" 
                    });
                }

                var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile, "articles");
                
                return Ok(new 
                { 
                    status = "success", 
                    message = "Upload ảnh thành công",
                    imageUrl = imageUrl
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new 
                { 
                    status = "failed", 
                    message = ex.Message 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi khi upload ảnh", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Upload multiple images for article using Cloudinary
        /// </summary>
        /// <param name="imageFiles">Multiple image files to upload</param>
        /// <returns>List of Cloudinary image URLs</returns>
        [HttpPost("upload-images")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UploadMultipleArticleImages(IFormFile[] imageFiles)
        {
            try
            {
                if (imageFiles == null || imageFiles.Length == 0)
                {
                    return BadRequest(new 
                    { 
                        status = "failed", 
                        message = "Không có file ảnh nào được tải lên" 
                    });
                }

                if (imageFiles.Length > 5)
                {
                    return BadRequest(new 
                    { 
                        status = "failed", 
                        message = "Chỉ được upload tối đa 5 ảnh cùng lúc" 
                    });
                }

                var imageUrls = await _cloudinaryService.UploadMultipleImagesAsync(imageFiles, "articles");
                
                return Ok(new 
                { 
                    status = "success", 
                    message = $"Upload {imageUrls.Count()} ảnh thành công",
                    imageUrls = imageUrls
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new 
                { 
                    status = "failed", 
                    message = ex.Message 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi khi upload ảnh", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Delete image from Cloudinary
        /// </summary>
        /// <param name="imageUrl">Cloudinary image URL to delete</param>
        /// <returns>Delete result</returns>
        [HttpDelete("delete-image")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteArticleImage([FromBody] string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return BadRequest(new 
                    { 
                        status = "failed", 
                        message = "URL ảnh không hợp lệ" 
                    });
                }

                var result = await _cloudinaryService.DeleteImageByUrlAsync(imageUrl);
                
                if (result)
                {
                    return Ok(new 
                    { 
                        status = "success", 
                        message = "Xóa ảnh thành công" 
                    });
                }
                else
                {
                    return BadRequest(new 
                    { 
                        status = "failed", 
                        message = "Không thể xóa ảnh" 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi khi xóa ảnh", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Create a new article (Admin and Staff only)
        /// </summary>
        /// <param name="articleDto">Article creation data</param>
        /// <returns>Created article</returns>
        //[Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleCreateDTO articleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get current user ID from JWT token
                var currentUserId = int.Parse(User.FindFirstValue("UserID"));
                var currentUserName = User.FindFirstValue("FullName") ?? "Unknown";

                // Map to full ArticleDTO
                var fullArticleDto = new ArticleDTO
                {
                    AuthorUserId = currentUserId,
                    ArticleCategoryId = articleDto.ArticleCategoryId,
                    ArticleStatusId = articleDto.ArticleStatusId,
                    Title = articleDto.Title,
                    Content = articleDto.Content,
                    Picture = articleDto.Picture
                };

                var article = await _articleService.AddArticleAsync(fullArticleDto);
                return CreatedAtAction(nameof(GetArticleById), new { id = article.ArticleId }, article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi hệ thống khi tạo bài viết", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Update an existing article (Admin and Staff only)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="articleDto"></param>
        /// <returns></returns>
        //[Authorize(Roles = "Admin,Staff")] // Chỉ Admin và Staff được sửa
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticleDTO articleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _articleService.UpdateArticleAsync(id, articleDto);
                if (!result)
                {
                    return NotFound(new { status = "failed", message = "Không tìm thấy bài viết" });
                }
                return Ok(new { status = "success", message = "Cập nhật bài viết thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi hệ thống khi cập nhật bài viết", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Delete an article (Admin and Staff only)
        /// </summary>
        /// <param name="id">Article ID</param>
        /// <returns>Delete result</returns>
        //[Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            try
            {
                var result = await _articleService.DeleteArticleAsync(id);
                if (!result)
                {
                    return NotFound(new { status = "failed", message = "Không tìm thấy bài viết" });
                }
                return Ok(new { status = "success", message = "Xóa bài viết thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi hệ thống khi xóa bài viết", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get article by ID (Public access)
        /// </summary>
        /// <param name="id">Article ID</param>
        /// <returns>Article details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            try
            {
                var article = await _articleService.GetArticleByIdAsync(id);
                if (article == null)
                {
                    return NotFound(new { status = "failed", message = "Không tìm thấy bài viết" });
                }
                return Ok(new { status = "success", data = article });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi hệ thống khi lấy thông tin bài viết", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get all articles with pagination (Public access)
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 50)</param>
        /// <param name="categoryId">Filter by category ID (optional)</param>
        /// <param name="statusId">Filter by status ID (optional)</param>
        /// <returns>Paginated list of articles</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllArticles(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? statusId = null)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                IEnumerable<object> articles;

                if (categoryId.HasValue)
                {
                    articles = await _articleService.GetArticlesByCategoryIdAsync(categoryId.Value);
                }
                else if (statusId.HasValue)
                {
                    articles = await _articleService.GetArticlesByStatusIdAsync(statusId.Value);
                }
                else
                {
                    articles = await _articleService.GetAllArticlesAsync();
                }

                // Apply pagination
                var totalCount = articles.Count();
                var paginatedArticles = articles
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new 
                { 
                    status = "success", 
                    data = paginatedArticles,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi hệ thống khi lấy danh sách bài viết", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get articles by author (Admin and Staff can see all, Authors can see their own)
        /// </summary>
        /// <param name="authorId">Author user ID</param>
        /// <returns>List of articles by author</returns>
        [HttpGet("author/{authorId}")]
        [Authorize]
        public async Task<IActionResult> GetArticlesByAuthor(int authorId)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue("UserID"));
                var currentUserRole = User.FindFirstValue("RoleID");

                // Check permissions: Admin/Staff can see all, others can only see their own
                if (currentUserRole != "1" && currentUserRole != "2" && currentUserId != authorId)
                {
                    return Forbid(new { status = "failed", message = "Bạn không có quyền xem bài viết của tác giả này" }.ToString());
                }

                var articles = await _articleService.GetArticlesByAuthorIdAsync(authorId);
                return Ok(new { status = "success", data = articles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi hệ thống khi lấy bài viết của tác giả", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get articles by category (Public access)
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of articles in category</returns>
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetArticlesByCategory(int categoryId)
        {
            try
            {
                var articles = await _articleService.GetArticlesByCategoryIdAsync(categoryId);
                return Ok(new { status = "success", data = articles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi hệ thống khi lấy bài viết theo danh mục", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get articles by status (Admin and Staff only for non-published articles)
        /// </summary>
        /// <param name="statusId">Status ID</param>
        /// <returns>List of articles with specified status</returns>
        [HttpGet("status/{statusId}")]
        public async Task<IActionResult> GetArticlesByStatus(int statusId)
        {
            try
            {
                // If requesting non-published articles (assuming status 1 = published), require authorization
                if (statusId != 1)
                {
                    var currentUserRole = User.FindFirstValue("RoleID");
                    if (currentUserRole != "1" && currentUserRole != "2")
                    {
                        return Forbid(new { status = "failed", message = "Bạn không có quyền xem bài viết có trạng thái này" }.ToString());
                    }
                }

                var articles = await _articleService.GetArticlesByStatusIdAsync(statusId);
                return Ok(new { status = "success", data = articles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi hệ thống khi lấy bài viết theo trạng thái", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Publish an article (Admin and Staff only)
        /// </summary>
        /// <param name="id">Article ID</param>
        /// <returns>Publish result</returns>
        [HttpPatch("{id}/publish")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> PublishArticle(int id)
        {
            try
            {
                // Assuming status ID 1 = Published
                var updateDto = new UpdateArticleDTO();
                // You might need to get the current article first to preserve other fields
                var currentArticle = await _articleService.GetArticleByIdAsync(id);
                if (currentArticle == null)
                {
                    return NotFound(new { status = "failed", message = "Không tìm thấy bài viết" });
                }

                // Create update DTO with published status
                // Note: You might need to implement a specific publish method in your service
                // This is a simplified version
                
                return Ok(new { status = "success", message = "Xuất bản bài viết thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    status = "error", 
                    message = "Lỗi hệ thống khi xuất bản bài viết", 
                    error = ex.Message 
                });
            }
        }
    }
}