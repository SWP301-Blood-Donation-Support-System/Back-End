using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class ArticleDTO
    {
        public int AuthorUserId { get; set; }
        public int ArticleCategoryId { get; set; }
        public int ArticleStatusId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Picture { get; set; }
    }
}
