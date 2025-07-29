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

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
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
        /// Create a new article (Admin and Staff only)
        /// </summary>
        /// <param name="articleDto">Article creation data</param>
        /// <returns>Created article</returns>
        [Authorize(Roles = "1,2")]
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
                var currentRoleId = int.Parse(User.FindFirstValue("RoleID"));
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
        [Authorize(Roles = "1,2")]
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
        /// Soft delete an article (Admin and Staff only) - Sets IsDeleted = true
        /// </summary>
        /// <param name="id">Article ID</param>
        /// <returns>Delete result</returns>
        [Authorize(Roles = "1,2")]
        [HttpPatch("{id}/delete")]
        public async Task<IActionResult> SoftDeleteArticle(int id)
        {
            try
            {
                var result = await _articleService.DeleteArticleAsync(id);
                if (!result)
                {
                    return NotFound(new { status = "failed", message = "Không tìm thấy bài viết" });
                }
                return Ok(new { status = "success", message = "Xóa bài viết thành công (soft delete)" });
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
                var currentUserRole = User.FindFirstValue(ClaimTypes.Role); // Use ClaimTypes.Role instead of "RoleID"

                // Check permissions: Admin/Staff can see all, others can only see their own
                if (currentUserRole != "Admin" && currentUserRole != "Staff" && currentUserId != authorId)
                {
                    return Forbid("Bạn không có quyền xem bài viết của tác giả này");
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
                    var currentUserRole = User.FindFirstValue(ClaimTypes.Role); // Use ClaimTypes.Role
                    if (currentUserRole != "Admin" && currentUserRole != "Staff")
                    {
                        return Forbid("Bạn không có quyền xem bài viết có trạng thái này");
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
                // Get current article to preserve other fields
                var currentArticle = await _articleService.GetArticleByIdAsync(id);
                if (currentArticle == null)
                {
                    return NotFound(new { status = "failed", message = "Không tìm thấy bài viết" });
                }

                // Create update DTO with published status (assuming status ID 1 = Published)
                var updateDto = new UpdateArticleDTO
                {
                    ArticleCategoryId = currentArticle.ArticleCategoryId,
                    ArticleStatusId = 1, // Published status
                    Title = currentArticle.Title,
                    Content = currentArticle.Content ?? "",
                    Picture = currentArticle.Picture
                };

                var result = await _articleService.UpdateArticleAsync(id, updateDto);
                if (!result)
                {
                    return BadRequest(new { status = "failed", message = "Không thể xuất bản bài viết" });
                }
                
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

    // DTO for storing image URL request  
    public class StoreImageUrlRequest
    {
        public string ImageUrl { get; set; } = string.Empty;
    }
}