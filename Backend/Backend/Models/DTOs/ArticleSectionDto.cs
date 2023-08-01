namespace Backend.Models.DTOs
{
    public class ArticleSectionDto
    {
        public string Id { get; set; }

        public string SectionTitle { get; set; }

        public string SectionImageUrl { get; set; }

        public string SectionText { get; set; }

        public int SectionNumber { get; set; }

        public string ArticleId { get; set; }
    }
}

