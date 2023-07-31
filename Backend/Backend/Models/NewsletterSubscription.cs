namespace Backend.Models
{
    public class NewsletterSubscription
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsSubscribed { get; set; } = true;
        public int ReceivedNewsletters { get; set; } = 0;

        public string? UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
