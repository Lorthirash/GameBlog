namespace Backend.Models.DTOs
{
    public class CreateSectionDto
    {
        public string ArticleId { get; set; } = string.Empty;
        public string SectionTitle { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public string SectionText { get; set; } = string.Empty;

        public int SectionNumber { get; set; }
    }
}

