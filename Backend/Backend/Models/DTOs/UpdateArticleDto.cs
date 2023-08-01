using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class UpdateArticleDto
    {
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Category { get; set; }
        [MaxLength(100, ErrorMessage = "description should be max 100 character")]
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }  
        public string Country { get; set; }
    }
}
