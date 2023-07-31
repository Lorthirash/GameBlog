namespace Backend.Models.Jwt
{
    public class AuthenticationResponse
    {
        public required JwtToken AccessToken { get; init; }
        public required JwtToken RefreshToken { get; init; }
    }
}
