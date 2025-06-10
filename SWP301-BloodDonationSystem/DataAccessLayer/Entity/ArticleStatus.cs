using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class ArticleStatus
{
    public int ArticleStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
