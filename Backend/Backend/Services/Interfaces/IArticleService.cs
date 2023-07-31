using Backend.Models;
using Backend.Models.DTOs;

namespace Backend.Services.Interfaces
{
    public interface IArticleService
    {
        Task<Article> CreateArticleAsync(CreateArticleDto createArticleDto, string currentUserId);
    }
}
