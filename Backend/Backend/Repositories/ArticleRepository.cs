using Backend.Database;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ArticleRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Article>> GetMyArticlesAsync(string myId)
        {
            var articles = await _applicationDbContext.Articles
                            .Where(a => a.UserId == myId)
                            .OrderByDescending(article => article.CreatedAt)
                            .ToListAsync();
            return articles;
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            var articles = await _applicationDbContext.Articles
                .Where(a => !a.IsDeleted)
                .OrderByDescending(article => article.CreatedAt)
                .ToListAsync();
            return articles;
        }
        public async Task<List<ArticleSection>> GetAllSectionByArticleAsync(string articleId)
        {
            return await _applicationDbContext.ArticleSections
               .Where(section => section.ArticleId == articleId)
               .ToListAsync();
        }
        public async Task<List<Article>> GetLatest3ArticlesAsync()
        {
            var articles = await _applicationDbContext.Articles
                            .Where(a => !a.IsDeleted)
                            .OrderByDescending(article => article.CreatedAt)
                            .Take(3)
                            .ToListAsync();
            return articles;
        }

        public async Task SaveArticleAsync(Article article)
        {
            await _applicationDbContext.Articles.AddAsync(article);
            await _applicationDbContext.SaveChangesAsync();
        }
        public async Task SaveSectionAsync(ArticleSection articleSection)
        {
            await _applicationDbContext.ArticleSections.AddAsync(articleSection);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<Article> GetArticleByIdAsync(string articleId)
        {
            return await _applicationDbContext.Articles
                .Where(article => article.ArticleId == articleId)
                .Include(articleSection => articleSection.ArticleSections)
                .SingleOrDefaultAsync()
                ?? throw new ArgumentException("Wrong Article Id");
        }

        public async Task<bool> RestoreArticleAsync(string articleId)
        {
            var article = await GetArticleByIdAsync(articleId);

            article.IsDeleted = false;
            article.LastModified = DateTime.UtcNow;
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteArticleAsync(string articleId)
        {
            var article = await GetArticleByIdAsync(articleId);


            article.IsDeleted = true;
            article.LastModified = DateTime.UtcNow;
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> HardDeleteArticleAsync(string articleId)
        {
            var article = await GetArticleByIdAsync(articleId);


            if (article == null)
            {
                throw new ArgumentException("Wrong Article Id");
            }

            _applicationDbContext.Articles.Remove(article);
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ArticleSection> GetSectionByIdAsync(string sectionId)
        {
            return await _applicationDbContext.ArticleSections
                 .Where(articleSection => articleSection.Id == sectionId)
                 .SingleOrDefaultAsync()
                 ?? throw new ArgumentException("Wrong Section Id");
        }

        public async Task<bool> HardDeleteSectionAsync(string sectionId)
        {
            var section = await GetSectionByIdAsync(sectionId);

            if (section == null) { throw new ArgumentException("Wrong section Id"); }

            _applicationDbContext.ArticleSections.Remove(section);
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }


    }
}
