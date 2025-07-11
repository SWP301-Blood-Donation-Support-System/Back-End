using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class ArticleRepository : GenericRepository<Article>, IArticleRepository
    {
        private readonly BloodDonationDbContext _context;
        public ArticleRepository(BloodDonationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Article>> GetArticlesByAuthorIdAsync(int authorId)
        {
            return await _context.Articles
                .Where(a => a.AuthorUserId == authorId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Article>> GetArticlesByCategoryIdAsync(int categoryId)
        {
            return await _context.Articles
                .Where(a => a.ArticleCategoryId == categoryId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Article>> GetArticlesByStatusIdAsync(int statusId)
        {
            return await _context.Articles
                .Where(a => a.ArticleStatusId == statusId)
                .ToListAsync();
        }
        public async Task<bool> SoftDeleteArticleAsync(int articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                return false;
            }
            article.IsDeleted = true; // Assuming IsDeleted is a property in the Article entity
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}