using Backend.Models;
using Backend.Models.Cloudinary;
using Backend.Models.DTOs;
using Backend.Models.EmailSettings;

namespace Backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDetailsDto>> GetAllUsersAsync();
        Task<bool> AddRoleToUser(string userId, string role);
        Task<bool> RemoveRoleFromUser(string userId, string role);
        Task<string> GetProfilePictureOfUserById(string currentUserId);
        Task<bool> CanUserChangeArticleAsync(string userId, string articleUserId);
        Task ChangeProfilePictureAsync(AddProfilePicture profilePicture, string currentUserId);
        Task<bool> CanUserChangeCommentAsync(string userId, string commentUserId);
        Task<string> GetCurrentUserNameAsync(string currentUserId);
        Task<(User newUser, string errorMessage)> CreateUserAsync(CreateUserDto createUser);
        Task<string> GenerateConfirmationLinkAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<List<EmailAddresses>> GetAllUserSubscribersAsync();
        Task<bool> CanUserCreateSectionAsync(string currentUserId, string articleId);

    }
}
