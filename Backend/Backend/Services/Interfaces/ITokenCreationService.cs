using Backend.Models;
using Backend.Models.Jwt;

namespace Backend.Services.Interfaces
{
    public interface ITokenCreationService
    {
        void ClearRefreshToken(string refreshToken);
        Task<AuthenticationResponse> CreateTokensAsync(User user);
        Task<AuthenticationResponse> RenewTokensAsync(string refreshToken);
    }

}
