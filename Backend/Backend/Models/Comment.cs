using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Comment
    {

        public string CommentId { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "username is required")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "username is required")]
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public int CommentLikeNumber { get; set; }
        public bool IsDeleted { get; set; } = false;

        //Foreign Key Article
        public string ArticleId { get; set; } = string.Empty;
        public Article? Article { get; set; }


        public string? UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
