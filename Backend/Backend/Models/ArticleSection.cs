namespace Backend.Models
{
    public class ArticleSection
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string SectionTitle { get; set; } = string.Empty;

        public string SectionImageUrl { get; set; }

        public string SectionText { get; set; } = string.Empty;

        public int SectionNumber { get; set; }



        public string ArticleId { get; set; } = string.Empty;
        public Article? Article { get; set; }
    }
}
