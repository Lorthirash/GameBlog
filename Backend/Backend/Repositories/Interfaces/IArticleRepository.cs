using Backend.Models;

namespace Backend.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        Task<bool> DeleteArticleAsync(string articleId);
        Task<List<Article>> GetAllArticlesAsync();
        Task<Article> GetArticleByIdAsync(string articleId);
        Task<List<Article>> GetLatest3ArticlesAsync();
        Task SaveArticleAsync(Article article);
        Task SaveChangesAsync();
        Task<List<Article>> GetMyArticlesAsync(string myId);
        Task<bool> RestoreArticleAsync(string articleId);
        Task<bool> HardDeleteArticleAsync(string articleId);
        Task SaveSectionAsync(ArticleSection articleSection);
        Task<ArticleSection> GetSectionByIdAsync(string sectionId);
        Task<bool> HardDeleteSectionAsync(string sectionId);
        Task<List<ArticleSection>> GetAllSectionByArticleAsync(string articleId);
    }
}
