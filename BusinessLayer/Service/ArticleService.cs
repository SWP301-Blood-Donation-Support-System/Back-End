using AutoMapper;
using BusinessLayer.IService;
using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;

        public ArticleService(IArticleRepository articleRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
        }

        public async Task<Article> AddArticleAsync(ArticleDTO articleDto)
        {
            var article = _mapper.Map<Article>(articleDto);
            article.CreatedAt = DateTime.Now;
            await _articleRepository.AddAsync(article);
            await _articleRepository.SaveChangesAsync();
            return article;
        }

        public async Task<bool> UpdateArticleAsync(int articleId, UpdateArticleDTO articleDto)
        {
            var existingArticle = await _articleRepository.GetByIdAsync(articleId);
            if (existingArticle == null)
            {
                return false;
            }
            _mapper.Map(articleDto, existingArticle);
            existingArticle.UpdatedAt = DateTime.Now;
            await _articleRepository.UpdateAsync(existingArticle);
            return await _articleRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteArticleAsync(int articleId)
        {
            return await _articleRepository.SoftDeleteArticleAsync(articleId);
        }

        public async Task<Article> GetArticleByIdAsync(int articleId)
        {
            return await _articleRepository.GetByIdAsync(articleId);
        }

        public async Task<IEnumerable<Article>> GetAllArticlesAsync()
        {
            return await _articleRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Article>> GetArticlesByAuthorIdAsync(int authorId)
        {
            return await _articleRepository.GetArticlesByAuthorIdAsync(authorId);
        }

        public async Task<IEnumerable<Article>> GetArticlesByCategoryIdAsync(int categoryId)
        {
            return await _articleRepository.GetArticlesByCategoryIdAsync(categoryId);
        }

        public async Task<IEnumerable<Article>> GetArticlesByStatusIdAsync(int statusId)
        {
            return await _articleRepository.GetArticlesByStatusIdAsync(statusId);
        }
    }
}