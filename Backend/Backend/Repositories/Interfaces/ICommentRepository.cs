using Backend.Models;

namespace Backend.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllCommentForBlogPostAsync(string blogPostId);
        Task SaveCommentAsync(Comment comment);

        Task<Comment> GetCommentByIdAsync(string commentId);
        Task SaveChangesAsync();
        Task<List<Comment>> GetAllCommentsOfUserAsync(string userId);
        Task<bool> DeleteArticleAsync(string commentId);
    }
}
