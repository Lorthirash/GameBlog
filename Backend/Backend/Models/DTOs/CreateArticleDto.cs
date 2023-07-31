using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class CreateArticleDto
    {
        [Required(ErrorMessage = "title is required"), MaxLength(60, ErrorMessage = "Title should be less than 60 character")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "text is required")]
        public string Text { get; set; } = string.Empty;
        public string? Category { get; set; }
        [Required(ErrorMessage = "description is required"), MaxLength(100, ErrorMessage = "description should be max 100 character")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Region is required")]   
        public string? Country { get; set; }
        public IFormFile? Image { get; set; }
    }
}
