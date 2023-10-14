namespace Backend.Models
{
    public class ArticleImage
    {
        public string ArticleImageId { get; set; } = Guid.NewGuid().ToString();

        public string ImageUrl { get; set; } = string.Empty;


        ////Foreign Key Article
        //public string ArticleId { get; set; } = string.Empty;
        //public Article? Article { get; set; }

    }
}
