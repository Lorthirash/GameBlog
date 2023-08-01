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

        [HttpGet("MyArticles")]
        [Authorize(Roles = "User")]   //My userRole
        public async Task<ActionResult<List<ArticleDetailsDto>>> GetMyArticlesAsync()
        {
            var currentUserId = User.GetCurrentUserId();

            try
            {
                List<ArticleDetailsDto> articles = await _articleService.GetMyArticlesAsync(currentUserId);
                return Ok(articles);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Unknow user.");
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while creating the article.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDetailsDto>> GetArticleByIdAsync(string id)
        {
            ArticleDetailsDto article = await _articleService.GetArticleByIdAsync(id);
            return Ok(article);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<ArticleListDetailsDto>>> GetAllArticlesAsync()
        {
            List<ArticleListDetailsDto> articles = await _articleService.GetAllArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("latest3")]
        public async Task<ActionResult<List<ArticleListDetailsDto>>> GetLatest3ArticlesAsync()
        {
            List<ArticleListDetailsDto> articles = await _articleService.GetLatest3ArticlesAsync();
            return Ok(articles);
        }


        [HttpPut("{articleId}")]
        [Authorize(Roles = "Journalist,Admin")]
        public async Task<ActionResult> UpdateArticleAsync([FromForm] UpdateArticleDto updateArticleDto, string articleId)
        {
            var currentUserId = User.GetCurrentUserId();
            try
            {
                await _articleService.UpdateArticleAsync(updateArticleDto, articleId, currentUserId);
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You do not have permission to update this article.");
            }
            catch (ArgumentException)
            {
                return NotFound("The requested article does not exist.");
            }
        }

        [HttpDelete("{articleId}")]
        [Authorize(Roles = "Journalist, Admin")]
        public async Task<ActionResult> DeleteArticleAsync(string articleId)
        {
            var currentUserId = User.GetCurrentUserId();
            try
            {
                bool isDeleted = await _articleService.TryDeletArticleAsync(articleId, currentUserId);
                if (isDeleted)
                {
                    return Ok();
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You do not have permission to delete this article.");
            }
            catch (ArgumentException)
            {
                return NotFound("This article id does not exist in the database.");
            }
            return BadRequest("An error occurred while deleting the article.");
        }


        [HttpPut("restore/{articleId}")]
        [Authorize(Roles = "Journalist, Admin")]
        public async Task<IActionResult> RestoreArticleAsync(string articleId)
        {
            var currentUserId = User.GetCurrentUserId();
            try
            {
                bool isRestored = await _articleService.RestoreArticleAsync(articleId, currentUserId);
                if (isRestored)
                {
                    return Ok();
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You do not have permission to restore this article.");
            }
            catch (ArgumentException)
            {
                return NotFound("This article id does not exist in the database.");
            }
            return BadRequest("An error occurred while restoring the article.");
        }

        [HttpDelete("hardDelete/{articleId}")]
        [Authorize(Roles = "Admin,Journalist")]
        public async Task<ActionResult> HardDeleteArticleAsync(string articleId)
        {
            var currentUserId = User.GetCurrentUserId();
            try
            {
                bool isDeleted = await _articleService.HardDeleteArticleAsync(articleId, currentUserId);
                if (isDeleted)
                {
                    return Ok();
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You do not have permission to delete this article.");
            }
            catch (ArgumentException)
            {
                return NotFound("This article id does not exist in the database.");
            }
            return BadRequest("An error occurred while deleting the article.");
        }

        [HttpPost("createSection")]
        [Authorize(Roles = "Journalist,Admin")]
        public async Task<ActionResult> CreateSectionAsync([FromForm] CreateSectionDto createSectionDto)
        {
            var currentUserId = User.GetCurrentUserId();
            var articleId = createSectionDto.ArticleId;

            try
            {
                await _articleService.CreateSectionAsync(createSectionDto, articleId, currentUserId);
                return Created("", createSectionDto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You are not authorized to create a section for this article.");
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Unknown article.");
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while creating the article.");
            }
        }

        [HttpGet("Sections{articleId}")]
        public async Task<ActionResult<List<SectionListDto>>> GetAllSectionByArticleAsync(string articleId)
        {
            List<SectionListDto> sections = await _articleService.GetAllSectionByArticleAsync(articleId);
            return Ok(sections);
        }

        [HttpPut("UpdateSection/{sectionId}")]
        [Authorize(Roles = "Journalist,Admin")]
        public async Task<ActionResult> UpdateSectionAsync([FromForm] UpdateSectionDto updateSectionDto, string sectionId)
        {
            var currentUserId = User.GetCurrentUserId();

            try
            {
                await _articleService.UpdateSectionAsync(updateSectionDto, sectionId, currentUserId);
                return Ok();

            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You do not have permission to update this section.");
            }
            catch (ArgumentException)
            {
                return NotFound("This section id does not exist in the database.");
            }

        }

        [HttpDelete("DeleteSection/{sectionId}")]
        [Authorize(Roles = "Admin,Journalist")]
        public async Task<ActionResult> DeleteSectionAsync(string sectionId)
        {
            var currentUserId = User.GetCurrentUserId();

            try
            {
                bool isDeleted = await _articleService.DeleteSectionAsync(sectionId, currentUserId);
                if (isDeleted)
                {
                    return Ok();
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You do not have permission to delete this section.");
            }
            catch (ArgumentException)
            {
                return NotFound("This section id does not exist in the database.");
            }
            return BadRequest("An error occurred while deleting the article.");
        }
    }
}
