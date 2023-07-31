using Backend.Models.EmailSettings;
using Org.BouncyCastle.Asn1.Pkcs;

namespace Backend.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendMailAsync(EmailData mailData);
        Task<bool> SendNewsletterAsync(List<EmailAddresses> addresses, EmailDetails details);
        Task<bool> SendConfirmationEmail(string userEmail, string link);
    }
}
