namespace Backend.Models.DTOs
{
    public class ArticleDetailsDto
    {
        public string ArticleId { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string LastModified { get; set; } = string.Empty;
        public int LikeNumber { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string UserId { get; set; } = string.Empty;

      
        public List<ArticleSectionDto> ArticleSections { get; set; }
    }
}
