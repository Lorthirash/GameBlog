using Backend.Database;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        
        private readonly ApplicationDbContext _applicationDbContext;

        public CommentRepository(ApplicationDbContext applicationDbContext)
        {          
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Comment>> GetAllCommentForBlogPostAsync(string artileId)
        {
            return await _applicationDbContext.Comments
                .Where(comment => comment.ArticleId == artileId)
                .Where(comment => !comment.IsDeleted)
                .Include(comment => comment.User)
                .ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(string commentId)
        {
            return await _applicationDbContext.Comments
                .Where(comment => comment.CommentId == commentId)
                .SingleOrDefaultAsync()
                ?? throw new ArgumentException("Wrong Comment Id");
        }

        public async Task<List<Comment>> GetAllCommentsOfUserAsync(string userId)
        {
            return await _applicationDbContext.Comments
                .Include(comment => comment.User)
                .Include(comment => comment.Article)
                .Where(comment => comment.UserId == userId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task SaveCommentAsync(Comment comment)
        {
            await _applicationDbContext.Comments.AddAsync(comment);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteArticleAsync(string commentId)
        {
            var comment = await _applicationDbContext.Comments
                .Where(comment => comment.CommentId.Equals(commentId))
                .SingleOrDefaultAsync()
            ?? throw new ArgumentException("Wrong Comment Id");

            comment.IsDeleted = true;
            comment.LastModified = DateTime.UtcNow;
            await SaveChangesAsync();
            return true;
        }
    }
}
