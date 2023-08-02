using Backend.Models.DTOs;

namespace Backend.Services.Interfaces
{
    public interface ICommentService
    {
        Task PostCommentAsync(CreateCommentDto createCommentDto, string articleId, string currentUserId);
        Task<List<CommentDetailsDto>> GetAllCommentForBlogPostAsync(string articleId);
        Task<CommentDetailsDto> GetCommentByIdAsync(string commentId);
        Task<List<UserCommentDto>> GetAllCommentsOfUserAsync(string userId);
        Task UpdateCommentAsync(UpdateCommentDto updateCommentDto, string commentId, string currentUserId);
        Task<bool> TryDeletCommentAsync(string commentId, string currentUserId);
    }
}
