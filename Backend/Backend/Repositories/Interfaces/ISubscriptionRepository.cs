using Backend.Models.EmailSettings;
using Backend.Models;

namespace Backend.Repositories.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<List<EmailAddresses>> GetNotUserSubscribersAsync();
        Task<NewsletterSubscription> SubscribeEmailToNewsletterAsync(string email);
        Task<NewsletterSubscription> SubscribeUserToNewsletterAsync(User newUser);
        Task SaveChangesAsync();
    }
}
