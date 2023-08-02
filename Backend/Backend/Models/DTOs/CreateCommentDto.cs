using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "commentText is required")]
        public string CommentText { get; set; } = string.Empty;
    }
}
