using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class ArticleCategory
{
    public int ArticleCategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
