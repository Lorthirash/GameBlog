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
        public async Task<List<ArticleDetailsDto>> GetMyArticlesAsync(string myId)
        {
            List<Article> articles = await _articleRepository.GetMyArticlesAsync(myId);
            if (articles is null) return null;

            return articles
            .Select(article => new ArticleDetailsDto
            {
                ArticleId = article.ArticleId,
                Author = article.AuthorName,
                Title = article.Title,
                Text = article.Text,
                Category = article.Category,
                Description = article.Description,
                ImageUrl = article.ImageUrl,            
                Country = article.Country,
                CreatedAt = article.CreatedAt.ToString(),
                LastModified = article.LastModified.ToString(),
                LikeNumber = article.ArticleLikeNumber,
                IsDeleted = article.IsDeleted,
                UserId = article.UserId,
            })
            .ToList();
        }
        public async Task<ArticleDetailsDto> GetArticleByIdAsync(string articleId)
        {
            Article article = await _articleRepository.GetArticleByIdAsync(articleId);

            if (article == null)
            {
                throw new ArgumentException("This article id does not exist in the database.");
            }

            if (article is null) return null;

            return new ArticleDetailsDto
            {
                ArticleId = article.ArticleId,
                Author = article.AuthorName,
                Title = article.Title,
                Text = article.Text,
                Category = article.Category,
                Description = article.Description,
                ImageUrl = article.ImageUrl,                
                Country = article.Country,
                CreatedAt = article.CreatedAt.ToString(),
                LastModified = article.LastModified.ToString(),
                LikeNumber = article.ArticleLikeNumber,
                IsDeleted = article.IsDeleted,
                UserId = article.UserId,
                ArticleSections = article.ArticleSections.Select(section => new ArticleSectionDto
                {
                    Id = section.Id,
                    SectionTitle = section.SectionTitle,
                    SectionImageUrl = section.SectionImageUrl,
                    SectionText = section.SectionText,
                    SectionNumber = section.SectionNumber,
                    ArticleId = section.ArticleId
                })
                .OrderBy(x => x.SectionNumber)
                .ToList(),
            };
        }

        public async Task<List<ArticleListDetailsDto>> GetAllArticlesAsync()
        {
            List<Article> articles = await _articleRepository.GetAllArticlesAsync();
            if (articles is null) return null;

            return articles
            .Select(article => new ArticleListDetailsDto
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Description = article.Description,
                ImageUrl = article.ImageUrl,
            })
            .ToList();
        }

        public async Task<List<ArticleListDetailsDto>> GetLatest3ArticlesAsync()
        {
            List<Article> articles = await _articleRepository.GetLatest3ArticlesAsync();
            if (articles is null) return null;

            return articles
            .Select(article => new ArticleListDetailsDto
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Description = article.Description,
                ImageUrl = article.ImageUrl,
            })
            .ToList();
        }

        public async Task UpdateArticleAsync(UpdateArticleDto updateArticleDto, string articleId, string currentUserId)
        {
            string imageUrl;
            Article article = await _articleRepository.GetArticleByIdAsync(articleId);
            if (updateArticleDto.Image != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(updateArticleDto.Image?.Name, updateArticleDto.Image?.OpenReadStream()),
                    PublicId = Guid.NewGuid().ToString(),
                    // az a folder, ahova Cloudinary-n rakja a képet
                    Folder = "Trawell"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                imageUrl = uploadResult.SecureUrl.ToString();

            }
            else
            {
                imageUrl = article.ImageUrl;
            }


            if (article == null)
            {
                throw new ArgumentException("This article id does not exist in the database.");
            }

            if (!await _userService.CanUserChangeArticleAsync(currentUserId, article.UserId))
            {
                throw new UnauthorizedAccessException("You do not have permission to update this article.");
            }

            article.Title = updateArticleDto.Title;
            article.Text = updateArticleDto.Text;
            article.Category = updateArticleDto.Category;
            article.Description = updateArticleDto.Description;
            article.ImageUrl = imageUrl;
            article.Country = updateArticleDto.Country;
            article.LastModified = DateTime.UtcNow;

            await _articleRepository.SaveChangesAsync();
        }

        public async Task<bool> TryDeletArticleAsync(string articleId, string currentUserId)
        {
            Article article = await _articleRepository.GetArticleByIdAsync(articleId);

            if (article == null)
            {
                throw new ArgumentException("This article id does not exist in the database.");
            }

            if (!await _userService.CanUserChangeArticleAsync(currentUserId, article.UserId))
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this article.");
            }
            ArgumentNullException.ThrowIfNull(articleId);
            return await _articleRepository.DeleteArticleAsync(articleId);
        }

        public async Task<bool> RestoreArticleAsync(string articleId, string currentUserId)
        {
            Article article = await _articleRepository.GetArticleByIdAsync(articleId);

            if (article == null)
            {
                throw new ArgumentException("This article id does not exist in the database.");
            }

            if (!await _userService.CanUserChangeArticleAsync(currentUserId, article.UserId))
            {
                throw new UnauthorizedAccessException("You do not have permission to restore this article.");
            }

            ArgumentNullException.ThrowIfNull(articleId);
            return await _articleRepository.RestoreArticleAsync(articleId);
        }

        public async Task<bool> HardDeleteArticleAsync(string articleId, string currentUserId)
        {
            Article article = await _articleRepository.GetArticleByIdAsync(articleId);

            if (article == null)
            {
                throw new ArgumentException("This article id does not exist in the database.");
            }

            if (!await _userService.CanUserChangeArticleAsync(currentUserId, article.UserId))
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this article.");
            }

            ArgumentNullException.ThrowIfNull(articleId);
            return await _articleRepository.HardDeleteArticleAsync(articleId);
        }

        public async Task CreateSectionAsync(CreateSectionDto createSectionDto, string articleId, string currentUserId)
        {

            if (!(await _userService.CanUserCreateSectionAsync(currentUserId, articleId)))
            {
                throw new UnauthorizedAccessException("You are not authorized to create a section for this article.");
            }

            string imageUrl;
            if (createSectionDto.Image != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(createSectionDto.Image?.Name, createSectionDto.Image?.OpenReadStream()),
                    PublicId = Guid.NewGuid().ToString(),
                    // az a folder, ahova Cloudinary-n rakja a képet
                    Folder = "Trawell"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                imageUrl = uploadResult.SecureUrl.ToString();

            }
            else
            {
                imageUrl = "";
            }

            ArticleSection articleSection = new ArticleSection()
            {
                ArticleId = articleId,
                SectionTitle = createSectionDto.SectionTitle,
                SectionImageUrl = imageUrl,
                SectionNumber = createSectionDto.SectionNumber,
                SectionText = createSectionDto.SectionText,
            };
            await _articleRepository.SaveSectionAsync(articleSection);
        }

        public async Task<List<SectionListDto>> GetAllSectionByArticleAsync(string articleId)
        {
            List<ArticleSection> section = await _articleRepository.GetAllSectionByArticleAsync(articleId);

            List<SectionListDto> articleSections = section.Select(section => new SectionListDto
            {
                Id = section.Id,
                SectionTitle = section.SectionTitle,
                SectionImageUrl = section.SectionImageUrl,
                SectionText = section.SectionText,
                SectionNumber = section.SectionNumber,


            }).ToList();

            return articleSections;
        }

        public async Task UpdateSectionAsync(UpdateSectionDto updateSectionDto, string sectionId, string currentUserId)
        {
            string imageUrl;
            var section = await _articleRepository.GetSectionByIdAsync(sectionId);
            if (updateSectionDto.Image != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(updateSectionDto.Image?.Name, updateSectionDto.Image?.OpenReadStream()),
                    PublicId = Guid.NewGuid().ToString(),
                    // az a folder, ahova Cloudinary-n rakja a képet
                    Folder = "Trawell"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                imageUrl = uploadResult.SecureUrl.ToString();
            }
            else
            {
                imageUrl = section.SectionImageUrl;
            }



            if (section == null)
            {
                throw new ArgumentException("This section id does not exist in the database.");
            }

            var article = await _articleRepository.GetArticleByIdAsync(section.ArticleId);
            if (!(await _userService.CanUserChangeArticleAsync(currentUserId, article.UserId)))
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this section.");
            }

            section.SectionTitle = updateSectionDto.SectionTitle;
            section.SectionImageUrl = imageUrl;
            section.SectionText = updateSectionDto.SectionText;
            section.SectionNumber = updateSectionDto.SectionNumber;

            await _articleRepository.SaveChangesAsync();
        }
        public async Task<bool> DeleteSectionAsync(string sectionId, string currentUserId)
        {
            var section = await _articleRepository.GetSectionByIdAsync(sectionId);

            if (section == null)
            {
                throw new ArgumentException("This section id does not exist in the database.");
            }

            var article = await _articleRepository.GetArticleByIdAsync(section.ArticleId);

            if (!(await _userService.CanUserChangeArticleAsync(currentUserId, article.UserId)))
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this section.");
            }

            return await _articleRepository.HardDeleteSectionAsync(sectionId);
        }
    }
}
