using Backend.Models;
using Backend.Models.DTOs;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Backend.Services
{
    public class ArticleService:IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly Cloudinary _cloudinary;
        private readonly IUserService _userService;

        public ArticleService(IArticleRepository articleRepository,Cloudinary cloudinary, IUserService userService)
        {
            _articleRepository = articleRepository;
            _cloudinary = cloudinary;
            _userService = userService;
        }

        public async Task<Article> CreateArticleAsync(CreateArticleDto createArticleDto, string currentUserId)
        {
            string imageUrl;
            if (createArticleDto.Image != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(createArticleDto.Image?.Name, createArticleDto.Image?.OpenReadStream()),
                    PublicId = Guid.NewGuid().ToString(),
                    Transformation = new Transformation().Width(1920).Height(1080).Gravity("auto").Crop("fill").Chain()
                    .Width("auto").Dpr("auto").Crop("scale"),
                    //Transformation.DefaultIsResponsive = true,
                    // az a folder, ahova Cloudinary-n rakja a képet
                    Folder = "Trawell"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                //var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                imageUrl = uploadResult.SecureUrl.ToString();

            }
            else
            {
                imageUrl = "https://res.cloudinary.com/dfwmtpwxf/image/upload/v1690801460/alexey-savchenko-k4Akpt5-Sfk-unsplash_1_pkkbqm.jpg";
            }
            var currentUserName = await _userService.GetCurrentUserNameAsync(currentUserId);

            Article article = new Article()
            {
                AuthorName = currentUserName,
                Title = createArticleDto.Title,
                Text = createArticleDto.Text,
                Category = createArticleDto.Category,
                Description = createArticleDto.Description,
                ImageUrl = imageUrl,              
                Country = createArticleDto.Country,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUserId,

            };
            await _articleRepository.SaveArticleAsync(article);

            return article;
        }
    }
}
