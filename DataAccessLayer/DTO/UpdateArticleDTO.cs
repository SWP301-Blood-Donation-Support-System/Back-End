using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTO
{
    public class UpdateArticleDTO
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
