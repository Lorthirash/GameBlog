using Backend.Models.Jwt;
using Backend.Models.Options;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Services.Interfaces;

namespace Backend.Services
{
    public class JwtService : ITokenCreationService
    {

        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<User> _userManager;
        private readonly IOptionsMonitor<JwtTokensOptions> _jwtTokensOptionsMonitor;

        public JwtService(IMemoryCache memoryCache, UserManager<User> userManager, IOptionsMonitor<JwtTokensOptions> jwtTokensOptionsMonitor)
        {
            _memoryCache = memoryCache;
            _userManager = userManager;
            _jwtTokensOptionsMonitor = jwtTokensOptionsMonitor;
        }

        public async Task<AuthenticationResponse> CreateTokensAsync(User user)
        {
            return new AuthenticationResponse
            {
                AccessToken = await CreateAccessTokenAsync(user),
                RefreshToken = CreateRefreshToken(user)
            };
        }

        private JwtToken CreateRefreshToken(User user)
        {

            JwtTokenOptions refreshTokenOptions = _jwtTokensOptionsMonitor.CurrentValue.RefreshTokenOptions;

            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            };

            JwtToken refreshToken = CreateToken(refreshTokenOptions, claims);

            _memoryCache.Set(refreshToken.Value, 0, refreshToken.Expiration);

            return refreshToken;
        }

        private async Task<JwtToken> CreateAccessTokenAsync(User user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            JwtTokenOptions accessTokenOptions = _jwtTokensOptionsMonitor.CurrentValue.AccessTokenOptions;

            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            }
            .Union(roles.Select(role => new Claim(ClaimTypes.Role, role)))
            .ToArray();

            return CreateToken(accessTokenOptions, claims);
        }

        private JwtToken CreateToken(JwtTokenOptions jwtTokenOptions, Claim[] claims)
        {
            var expiration = DateTime.UtcNow.AddMinutes(jwtTokenOptions.ExpirationMinutes);

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenOptions.Key)),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtTokenOptions.Issuer,
                audience: jwtTokenOptions.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return new JwtToken
            {
                Value = tokenHandler.WriteToken(token),
                Expiration = expiration
            };
        }

        public async Task<AuthenticationResponse> RenewTokensAsync(string refreshToken)
        {
            if (!_memoryCache.TryGetValue(refreshToken, out var _))
            {
                throw new JwtException($"Refresh token is missing: {refreshToken}");
            }

            JwtTokenOptions refreshTokenOptions = _jwtTokensOptionsMonitor.CurrentValue.RefreshTokenOptions;

            SecurityToken validatedToken;
            ClaimsPrincipal claimsPrincipal;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshTokenOptions.Key)),
                ValidAudience = refreshTokenOptions.Audience,
                ValidIssuer = refreshTokenOptions.Issuer,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true
            };

            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(
                    refreshToken, tokenValidationParameters, out validatedToken);
            }
            catch (SecurityTokenException exception)
            {
                throw new JwtException("JWT token validation failed.", exception);
            }

            User user = await _userManager.GetUserAsync(claimsPrincipal)
                ?? throw new InvalidOperationException(
                    $"User not found with id {claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)}");

            return new AuthenticationResponse
            {
                AccessToken = await CreateAccessTokenAsync(user),
                RefreshToken = CreateRefreshToken(user)
            };
        }

        public void ClearRefreshToken(string refreshToken)
        {
            _memoryCache.Remove(refreshToken);
        }
    }
}
