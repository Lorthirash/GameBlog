namespace Backend.Models.Jwt
{
    public class JwtToken
    {
        public required string Value { get; set; }
        public required DateTime Expiration { get; set; }
    }
}
