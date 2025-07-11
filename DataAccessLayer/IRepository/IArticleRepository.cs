
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IArticleRepository : IGenericRepository<Article>
    {
        Task<IEnumerable<Article>> GetArticlesByAuthorIdAsync(int authorId);
        Task<IEnumerable<Article>> GetArticlesByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Article>> GetArticlesByStatusIdAsync(int statusId);
        Task<bool> SoftDeleteArticleAsync(int articleId);
    }
}
