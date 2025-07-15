using BusinessLayer.IService;
using DataAccessLayer.DTO;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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

        //[Authorize(Roles = "Admin,Staff")] // Chỉ Admin và Staff được tạo
        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleDTO articleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var article = await _articleService.AddArticleAsync(articleDto);
                return CreatedAtAction(nameof(GetArticleById), new { id = article.ArticleId }, article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update the article by the Id and the content in the body
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
            var result = await _articleService.UpdateArticleAsync(id, articleDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        //[Authorize(Roles = "Admin,Staff")] // Chỉ Admin và Staff được xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var result = await _articleService.DeleteArticleAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArticles()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetArticlesByAuthor(int authorId)
        {
            var articles = await _articleService.GetArticlesByAuthorIdAsync(authorId);
            return Ok(articles);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetArticlesByCategory(int categoryId)
        {
            var articles = await _articleService.GetArticlesByCategoryIdAsync(categoryId);
            return Ok(articles);
        }

        [HttpGet("status/{statusId}")]
        public async Task<IActionResult> GetArticlesByStatus(int statusId)
        {
            var articles = await _articleService.GetArticlesByStatusIdAsync(statusId);
            return Ok(articles);
        }
    }
}