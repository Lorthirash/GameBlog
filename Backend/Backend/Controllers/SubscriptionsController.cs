using Backend.Models.EmailSettings;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscribtionService _subService;

        public SubscriptionsController(ISubscribtionService subService)
        {
            _subService = subService;
        }

        [HttpPost("SubscribeWithEmail")]
        public async Task<ActionResult> SubscribeOnlyWithEmail(EmailDto subscription)
        {
            await _subService.SubscribeEmailToNewsletterAsync(subscription.Email);

            return Ok();
        }


    }
}
