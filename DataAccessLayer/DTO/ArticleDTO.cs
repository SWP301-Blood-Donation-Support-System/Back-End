using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTO
{
    public class ArticleDTO
    {
        [Required(ErrorMessage = "Author User ID is required")]
        public int AuthorUserId { get; set; }

        [Required(ErrorMessage = "Article Category ID is required")]
        public int ArticleCategoryId { get; set; }

        [Required(ErrorMessage = "Article Status ID is required")]
        public int ArticleStatusId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Url(ErrorMessage = "Picture must be a valid URL")]
        public string? Picture { get; set; }
    }

    public class ArticleResponseDTO
    {
        public int ArticleId { get; set; }
        public int AuthorUserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int ArticleCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ArticleStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Picture { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ArticleCreateDTO
    {
        [Required(ErrorMessage = "Article Category ID is required")]
        public int ArticleCategoryId { get; set; }

        [Required(ErrorMessage = "Article Status ID is required")]
        public int ArticleStatusId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Url(ErrorMessage = "Picture must be a valid URL")]
        public string? Picture { get; set; }
    }
}
