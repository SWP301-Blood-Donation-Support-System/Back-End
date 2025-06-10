using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class Article
{
    public int ArticleId { get; set; }

    public int AuthorUserId { get; set; }

    public int ArticleCategoryId { get; set; }

    public int ArticleStatusId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? Picture { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ArticleCategory ArticleCategory { get; set; } = null!;

    public virtual ArticleStatus ArticleStatus { get; set; } = null!;

    public virtual User AuthorUser { get; set; } = null!;
}
