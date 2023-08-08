using Backend.Models.EmailSettings;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly ISubscribtionService _subscribeService;

        public EmailController(IEmailService emailService, IUserService userService, ISubscribtionService subscribeService)
        {
            _emailService = emailService;
            _userService = userService;
            _subscribeService = subscribeService;
        }

        [HttpPost]
        [Route("SendMail")]
        public async Task<bool> SendMailAsync(EmailData mailData)
        {
            return await _emailService.SendMailAsync(mailData);
        }

        [HttpPost]
        [Route("SendNewsletter")]
        public async Task<ActionResult> SendNewsletterAsync(EmailDetails details)
        {
            List<EmailAddresses> addresses = await _userService.GetAllUserSubscribersAsync();
            addresses = await _subscribeService.AddNotUserSubscibersAsync(addresses);
            try
            {
                Console.WriteLine(addresses.ToString());
                await _emailService.SendNewsletterAsync(addresses, details);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Ok();
        }
    }
}
