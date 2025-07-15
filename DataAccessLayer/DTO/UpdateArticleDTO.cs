using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class UpdateArticleDTO
    {
        public int ArticleCategoryId { get; set; }
        public int ArticleStatusId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Picture { get; set; }
    }
}
