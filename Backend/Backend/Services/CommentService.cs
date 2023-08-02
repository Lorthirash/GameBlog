using Backend.Models;
using Backend.Models.DTOs;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services
{
    public class CommentService:ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IUserService _userService;

        public CommentService(ICommentRepository commentRepository, IArticleRepository articleRepository, IUserService userService)
        {
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
            _userService = userService;
        }


        public async Task PostCommentAsync(CreateCommentDto createCommentDto, string articleId, string currentUserId)
        {
            var currentUserName = await _userService.GetCurrentUserNameAsync(currentUserId);
            Article article = await _articleRepository.GetArticleByIdAsync(articleId);

            Comment comment = new Comment
            {
                UserName = currentUserName,
                CommentText = createCommentDto.CommentText,
                CreatedAt = DateTime.UtcNow,
                ArticleId = articleId,
                UserId = currentUserId

            };


            await _commentRepository.SaveCommentAsync(comment);
        }
        public async Task<List<CommentDetailsDto>> GetAllCommentForBlogPostAsync(string articleId)
        {
            List<Comment> comments = await _commentRepository.GetAllCommentForBlogPostAsync(articleId);

            List<CommentDetailsDto> result = comments.Select(comment => new CommentDetailsDto
            {
                CommentId = comment.CommentId,
                Author = comment.UserName,
                CommentText = comment.CommentText,
                CreatedAt = comment.CreatedAt.ToString(),
                LastModifiedAt = comment.LastModified.ToString(),
                UserId = comment.UserId,
                ProfilePictureUrl = comment.User.ProfilePictureUrl,
                IsDeleted = comment.IsDeleted,

            }).ToList();

            return result;
        }

        public async Task<CommentDetailsDto> GetCommentByIdAsync(string commentId)
        {
            Comment comment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (comment is null) { return null; }


            return new CommentDetailsDto
            {
                CommentId = comment.CommentId,
                Author = comment.UserName,
                CommentText = comment.CommentText,
                CreatedAt = comment.CreatedAt.ToString(),
                LastModifiedAt = comment.LastModified.ToString(),
                UserId = comment.UserId,


            };
        }

        public async Task<List<UserCommentDto>> GetAllCommentsOfUserAsync(string userId)
        {
            List<Comment> comments = await _commentRepository.GetAllCommentsOfUserAsync(userId);

            List<UserCommentDto> userComments = comments.Select(comment => new UserCommentDto
            {
                ArticleId = comment.ArticleId,
                ArticleTitle = comment.Article.Title,
                CommentId = comment.CommentId,
                CommentText = comment.CommentText,
                CreatedAt = comment.CreatedAt.ToString(),
                IsDeleted = comment.IsDeleted
            }).ToList();

            return userComments;
        }

        public async Task UpdateCommentAsync(UpdateCommentDto updateCommentDto, string commentId, string currentUserId)
        {
            Comment comment = await _commentRepository.GetCommentByIdAsync(commentId);

            if (!await _userService.CanUserChangeCommentAsync(currentUserId, comment.UserId))
            {
                throw new UnauthorizedAccessException("You do not have permission to update this comment.");
            }

            comment.CommentText = updateCommentDto.CommentText;
            comment.LastModified = DateTime.UtcNow;

            await _commentRepository.SaveChangesAsync();
        }

        public async Task<bool> TryDeletCommentAsync(string commentId, string currentUserId)
        {
            Comment comment = await _commentRepository.GetCommentByIdAsync(commentId);

            if (!await _userService.CanUserChangeCommentAsync(currentUserId, comment.UserId))
            {
                throw new UnauthorizedAccessException("You do not have permission to update this comment.");
            }
            ArgumentNullException.ThrowIfNull(commentId);
            return await _commentRepository.DeleteArticleAsync(commentId);
        }
    }
}

