using Backend.Models.EmailSettings;
using Backend.Models;

namespace Backend.Services.Interfaces
{
    public interface ISubscribtionService
    {
        Task<List<EmailAddresses>> AddNotUserSubscibersAsync(List<EmailAddresses> addresses);
        Task SubscribeEmailToNewsletterAsync(string email);
        Task SubscribeUserToNewsletterAsync(User newUser);
    }
}
