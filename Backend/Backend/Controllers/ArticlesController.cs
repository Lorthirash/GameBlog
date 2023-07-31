using Backend.Extensions;
using Backend.Models.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }


        [HttpPost("createArticle")]
        [Authorize(Roles = "Journalist,Admin")]
        public async Task<ActionResult> CreateArticleAsync([FromForm] CreateArticleDto createArticleDto)
        {
            var currentUserId = User.GetCurrentUserId();

            try
            {
                var article = await _articleService.CreateArticleAsync(createArticleDto, currentUserId);
                return Created("", article);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Unknown user.");
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while creating the article.");
            }
        }
    }
}
