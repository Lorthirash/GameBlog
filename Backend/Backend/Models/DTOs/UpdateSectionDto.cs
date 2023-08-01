namespace Backend.Models.DTOs
{
    public class UpdateSectionDto
    {
        public string SectionTitle { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public string SectionText { get; set; } = string.Empty;

        public int SectionNumber { get; set; }
    }
}
