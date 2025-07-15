using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetAllArticlesAsync();
        Task<Article> GetArticleByIdAsync(int id);
        Task<Article> AddArticleAsync(ArticleDTO articleDto);
        Task<bool> UpdateArticleAsync(int id, UpdateArticleDTO updateArticleDto);
        Task<IEnumerable<Article>> GetArticlesByAuthorIdAsync(int authorId);
        Task<IEnumerable<Article>> GetArticlesByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Article>> GetArticlesByStatusIdAsync(int statusId);
        Task<bool> DeleteArticleAsync(int articleId);
    }
}
