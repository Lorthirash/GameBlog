using Backend.Models;
using Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Services.Interfaces
{
    public interface IArticleService
    {
        Task<Article> CreateArticleAsync(CreateArticleDto createArticleDto, string currentUserId);
        Task<List<ArticleDetailsDto>> GetMyArticlesAsync(string myId);     
        Task<ArticleDetailsDto> GetArticleByIdAsync(string articleId);
        Task<List<ArticleListDetailsDto>> GetAllArticlesAsync();
        Task<List<ArticleListDetailsDto>> GetLatest3ArticlesAsync();
        Task UpdateArticleAsync(UpdateArticleDto updateArticleDto, string articleId, string currentUserId);
        Task<bool> TryDeletArticleAsync(string articleId, string currentUserId);
        Task<bool> RestoreArticleAsync(string articleId, string currentUserId);
        Task<bool> HardDeleteArticleAsync(string articleId, string currentUserId);
        Task CreateSectionAsync(CreateSectionDto createSectionDto, string articleId, string currentUserId);
        Task<List<SectionListDto>> GetAllSectionByArticleAsync(string articleId);
        Task UpdateSectionAsync(UpdateSectionDto updateSectionDto, string sectionId, string currentUserId);
        Task<bool> DeleteSectionAsync(string sectionId, string currentUserId);

    }
}
