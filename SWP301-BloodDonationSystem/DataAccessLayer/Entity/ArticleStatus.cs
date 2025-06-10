using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class ArticleStatus
{
    public int ArticleStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
