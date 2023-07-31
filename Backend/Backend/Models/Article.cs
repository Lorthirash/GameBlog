namespace Backend.Models
{
    public class Article
    {
        public string ArticleId { get; set; } = Guid.NewGuid().ToString();

        public string AuthorName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
        public string? Category { get; set; }

        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; }
      
        public string? Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
        public int ArticleLikeNumber { get; set; }
        public bool IsDeleted { get; set; } = false;

        public List<Comment> Comments { get; set; } = new();
        //public List<Photo> Images { get; set; } = new();


        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }


        public List<ArticleSection> ArticleSections { get; set; } = new();

    }
}
