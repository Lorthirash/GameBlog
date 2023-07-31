namespace Backend.Models.DTOs
{
    public class UserInfoDto
    {
        public required string UserId { get; init; }
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public required IEnumerable<string> Roles { get; init; }
        public required string ProfilePictureUrl { get; init; }
    }
}
