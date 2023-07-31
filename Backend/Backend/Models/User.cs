using Microsoft.AspNetCore.Identity;

namespace Backend.Models
{
    public class User: IdentityUser
    {

        public List<Article> Articles { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public NewsletterSubscription? Subscription { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
