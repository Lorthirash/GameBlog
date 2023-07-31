namespace Backend.Models.Jwt
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
