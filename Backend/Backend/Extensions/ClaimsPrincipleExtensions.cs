using System.Security.Claims;

namespace Backend.Extensions
{
    internal static class ClaimsPrincipleExtensions
    {
        public static string GetCurrentUserId(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);
            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Current user Id is null");
        }
    }
}
