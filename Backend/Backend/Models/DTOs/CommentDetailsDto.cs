namespace Backend.Models.DTOs
{
    public class CommentDetailsDto
    {
        public string CommentId { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CommentText { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string LastModifiedAt { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
