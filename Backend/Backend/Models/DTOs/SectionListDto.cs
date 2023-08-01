namespace Backend.Models.DTOs
{
    public class SectionListDto
    {
        public string Id { get; set; }

        public string SectionTitle { get; set; } = string.Empty;

        public string SectionImageUrl { get; set; }

        public string SectionText { get; set; } = string.Empty;

        public int SectionNumber { get; set; }

    }
}
