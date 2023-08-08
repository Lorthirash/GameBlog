using Backend.Database;
using Backend.Models.EmailSettings;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public SubscriptionRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<List<EmailAddresses>> GetNotUserSubscribersAsync()
        {
            List<NewsletterSubscription> subscribers = await _applicationDbContext.NewsletterSubscriptions
                .Where(subscriber => subscriber.UserId == null)
                .Select(subscriber => subscriber)
                .ToListAsync();

            List<EmailAddresses> addressList = new List<EmailAddresses>();
            foreach (var subscriber in subscribers)
            {
                string[] name = subscriber.Email.Split('@');
                string userName = name[0];
                addressList.Add(new EmailAddresses
                {
                    EmailToId = subscriber.Email,
                    EmailToName = userName
                });
            }
            return addressList;
        }

        public async Task<NewsletterSubscription> SubscribeEmailToNewsletterAsync(string email)
        {
            var subscription = new NewsletterSubscription()
            {
                Email = email,
                UserId = null
            };
            await _applicationDbContext.NewsletterSubscriptions.AddAsync(subscription);
            return subscription;
        }

        public async Task<NewsletterSubscription> SubscribeUserToNewsletterAsync(User newUser)
        {
            var subscription = new NewsletterSubscription()
            {
                Email = newUser.Email!,
                UserId = newUser.Id
            };
            await _applicationDbContext.NewsletterSubscriptions.AddAsync(subscription);
            return subscription;
            
        }

        public async Task SaveChangesAsync()
        {           
            await _applicationDbContext.SaveChangesAsync();
        }

       
    }
}
