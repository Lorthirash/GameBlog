using Backend.Models.EmailSettings;
using Backend.Models;
using Backend.Services.Interfaces;
using Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class SubscribeService : ISubscribtionService
    {
        private readonly ApplicationDbContext _dbContext;

        public SubscribeService(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public async Task<List<EmailAddresses>> AddNotUserSubscibersAsync(List<EmailAddresses> addresses)
        {
            List<NewsletterSubscription> subscribers = await _dbContext.NewsletterSubscriptions
                            .Where(subscriber => subscriber.UserId == null)
                            .Select(subscriber => subscriber)
                            .ToListAsync();

            List<EmailAddresses> addresslist = addresses;
            foreach (var subscriber in subscribers)
            {
                string[] name = subscriber.Email.Split('@');
                string userName = name[0];
                addresslist.Add(new EmailAddresses
                {
                    EmailToId = subscriber.Email,
                    EmailToName = userName
                });
            }
            return addresslist;
        }

        public async Task SubscribeEmailToNewsletterAsync(string email)
        {
            var subscription = new NewsletterSubscription()
            {
                Email = email,
                UserId = null
            };

            await _dbContext.NewsletterSubscriptions.AddAsync(subscription);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SubscribeUserToNewsletterAsync(User newUser)
        {
            var subscription = new NewsletterSubscription()
            {
                Email = newUser.Email!,
                UserId = newUser.Id
            };

            await _dbContext.NewsletterSubscriptions.AddAsync(subscription);
            await _dbContext.SaveChangesAsync();
        }
    }
}
