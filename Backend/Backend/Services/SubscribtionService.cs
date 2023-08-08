using Backend.Models;
using Backend.Models.EmailSettings;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services
{
    public class SubscribtionService : ISubscribtionService
    {
        
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscribtionService(ISubscriptionRepository subscriptionRepository)
        {
           
            _subscriptionRepository = subscriptionRepository;
        }     

        public async Task<List<EmailAddresses>> AddNotUserSubscibersAsync(List<EmailAddresses> addresses)
        {
            List<EmailAddresses> addressList = await _subscriptionRepository.GetNotUserSubscribersAsync();
            // Műveletek a listával
            return addressList;
        }

        public async Task SubscribeEmailToNewsletterAsync(string email)
        {
            await _subscriptionRepository.SubscribeEmailToNewsletterAsync(email);
            await _subscriptionRepository.SaveChangesAsync();
        }

        public async Task SubscribeUserToNewsletterAsync(User newUser)
        {
            await _subscriptionRepository.SubscribeUserToNewsletterAsync(newUser);
            await _subscriptionRepository.SaveChangesAsync();
        }
    }
}
