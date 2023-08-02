using Backend.Extensions;
using Backend.Models;
using Backend.Models.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("{articleId}")]
        [Authorize(Roles = "User,Journalist,Admin")]
        public async Task<ActionResult> PostCommentAsync(CreateCommentDto createCommentDto, string articleId)
        {
            var currentUserId = User.GetCurrentUserId();
            await _commentService.PostCommentAsync(createCommentDto, articleId, currentUserId);
            return Created("", createCommentDto);
        }

        [HttpGet("{articleId}")]
        public async Task<ActionResult<List<CommentDetailsDto>>> GetAllCommentForBlogPost(string articleId)
        {
            List<CommentDetailsDto> result = await _commentService.GetAllCommentForBlogPostAsync(articleId);
            return Ok(result);
        }

        [HttpGet("GetCommentById/{commentId}")]
        public async Task<ActionResult<CommentDetailsDto>> GetCommentByIdAsync(string commentId)
        {
            CommentDetailsDto comment = await _commentService.GetCommentByIdAsync(commentId);
            return Ok(comment);
        }

        [HttpGet("GetAllCommentsOfUser/{userId}")]
        public async Task<ActionResult<IEnumerable<UserCommentDto>>> GetAllCommentsOfUserAsync(string userId)
        {
            if (User.GetCurrentUserId() != userId)
            {
                return BadRequest("ID mismatch");
            }

            List<UserCommentDto> userComments = await _commentService.GetAllCommentsOfUserAsync(userId);
            return Ok(userComments);
        }

        [HttpPut("{commentId}")]
        [Authorize(Roles = "User,Journalist,Admin")]
        public async Task<ActionResult> UpdateCommentAsync(UpdateCommentDto updateCommentDto, string commentId)
        {

            var currentUserId = User.GetCurrentUserId();
            try
            {
                await _commentService.UpdateCommentAsync(updateCommentDto, commentId, currentUserId);
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You do not have permission to update this comment.");
            }
            catch (ArgumentException)
            {
                return NotFound("The requested comment does not exist.");
            }

        }

        [HttpDelete("{commentId}")]
        [Authorize(Roles = "User,Journalist,Admin")]
        public async Task<ActionResult> DeleteCommentAsync(string commentId)
        {
            var currentUserId = User.GetCurrentUserId();
            try
            {
                bool isDeleted = await _commentService.TryDeletCommentAsync(commentId, currentUserId);
                if (isDeleted)
                {
                    return Ok();
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You do not have permission to delete this comment.");
            }
            catch (ArgumentException)
            {
                return NotFound("This comment id does not exist in the database.");
            }
            return BadRequest("An error occurred while deleting the comment.");
        }

    }
}
