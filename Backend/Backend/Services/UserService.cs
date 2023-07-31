using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Backend.Database;
using Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Backend.Models.Cloudinary;
using Backend.Models.EmailSettings;

namespace Backend.Services
{

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<User> _userManager;
        private readonly Cloudinary _cloudinary;
        private readonly IMemoryCache _memoryCache;
        private readonly IArticleRepository _articleRepository;


        public UserService(ApplicationDbContext applicationDbContext, UserManager<User> userManager, Cloudinary cloudinary, IMemoryCache memoryCache, IArticleRepository articleRepository)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _cloudinary = cloudinary;
            _memoryCache = memoryCache;
            _articleRepository = articleRepository;


        }


        public async Task<IEnumerable<UserDetailsDto>> GetAllUsersAsync()
        {
            var users = await _applicationDbContext.Users.AsNoTracking().ToArrayAsync();

            List<UserDetailsDto> userListViews = new();
            foreach (var user in users)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                userListViews.Add(new UserDetailsDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles
                });
            }
            return userListViews;
        }

        public async Task<bool> AddRoleToUser(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false; // Felhasználó nem található
            }

            var result = await _userManager.AddToRoleAsync(user, role);

            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleFromUser(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false; // Felhasználó nem található
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role);

            return result.Succeeded;
        }

        public async Task<string> GetProfilePictureOfUserById(string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(currentUserId);

            return user.ProfilePictureUrl;
        }

        public async Task<bool> CanUserChangeArticleAsync(string userId, string articleUserId)
        {
            if (articleUserId != userId)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Admin")))
                {
                    return false;
                }
            }

            return true;
        }
        public async Task<bool> CanUserChangeCommentAsync(string userId, string commentUserId)
        {
            if (commentUserId != userId)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Admin")))
                {
                    return false;
                }
            }

            return true;
        }
        public async Task<bool> CanUserCreateSectionAsync(string currentUserId, string articleId)
        {
            // Ellenőrizzük hogy az aktuális felhasználó-e a cikk szerzője
            var article = await _articleRepository.GetArticleByIdAsync(articleId);
            if (article.UserId != currentUserId && !(await CanUserChangeArticleAsync(currentUserId, article.UserId)))
            {
                return false;
            }

            return true;
        }

        public async Task ChangeProfilePictureAsync(AddProfilePicture profilePicture, string currentUserId)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(profilePicture.Image?.Name, profilePicture.Image?.OpenReadStream()),
                PublicId = Guid.NewGuid().ToString(),
                Transformation = new Transformation().Width(400).Height(400).Gravity("face").Crop("fill").Chain()
                //.Radius(5).Chain()
                .Width("auto").Dpr("auto").Crop("scale"),
                // az a folder, ahova Cloudinary-n rakja a képet
                Folder = "ProfilePictures"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            var imageUrl = uploadResult.SecureUrl.ToString();

            var user = await _userManager.FindByIdAsync(currentUserId);

            user.ProfilePictureUrl = imageUrl;

            await _userManager.UpdateAsync(user);
        }
        public async Task<string> GetCurrentUserNameAsync(string currentUserId)
        {
            var userName = await _applicationDbContext.Users
                .Where(user => user.Id == currentUserId)
                .Select(user => user.UserName)
                .SingleAsync();

            if (userName == null)
            {
                throw new ArgumentNullException("Unknow user.");
            }
            return userName;


        }
        public async Task<(User newUser, string errorMessage)> CreateUserAsync(CreateUserDto createUser)
        {
            var newUser = new User()
            {
                UserName = createUser.UserName,
                Email = createUser.Email,
                ProfilePictureUrl = "https://res.cloudinary.com/dzprbzfjm/image/upload/v1684929800/Trawell/d7a9621b-20b9-4af2-8985-a4a5b8b59a26.png"
            };

            var userNameTaken = await _userManager.FindByNameAsync(newUser.UserName);
            var emailTaken = await _userManager.FindByEmailAsync(newUser.Email);

            if (userNameTaken != null)
            {
                return (null, "This username is already taken");
            }

            if (emailTaken != null)
            {
                return (null, "This email address is already taken");
            }

            var result = await _userManager.CreateAsync(newUser, createUser.Password);

            if (!result.Succeeded)
            {
                return (null, "Failed to create user");
            }

            var defaultRole = "User";
            var addToRoleResult = await _userManager.AddToRoleAsync(newUser, defaultRole);

            if (!addToRoleResult.Succeeded)
            {
                return (null, "Failed to assign role to user");
            }

            return (newUser, null);
        }

        public async Task<string> GenerateConfirmationLinkAsync(User user)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Itt visszatérünk az e-mail megerősítési tokennel, és a controllerben fogjuk generálni a linket

            _memoryCache.Set(emailConfirmationToken, DateTime.Now);

            return emailConfirmationToken;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            _memoryCache.TryGetValue(token, out DateTime tokenCreationDate);

            if (tokenCreationDate > DateTime.Now.AddMinutes(-15))
            {
                var user = await _userManager.FindByIdAsync(userId);
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    _memoryCache.Remove(token);
                    return true;
                }
            }
            // user törlése?
            return false;
        }

        public async Task<List<EmailAddresses>> GetAllUserSubscribersAsync()
        {
            List<User> subscribers = await _userManager.Users.Include("Subscription")
                .Where(user => user.Subscription.IsSubscribed)
                .Select(user => user)
                .ToListAsync();

            List<EmailAddresses> addresses = new();
            foreach (var subscriber in subscribers)
            {
                addresses.Add(new EmailAddresses
                {
                    EmailToId = subscriber.Email,
                    EmailToName = subscriber.UserName
                });
            }
            return addresses;
        }


    }
}
