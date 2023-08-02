namespace Backend.Models.DTOs
{
    public class UserCommentDto
    {
        public string CommentId { get; set; } = string.Empty;
        public string CommentText { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string ArticleId { get; set; } = string.Empty;
        public string ArticleTitle { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
