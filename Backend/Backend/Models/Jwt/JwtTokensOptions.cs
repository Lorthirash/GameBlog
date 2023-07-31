namespace Backend.Models.Options
{
    public class JwtTokensOptions
    {

        public JwtTokenOptions AccessTokenOptions { get; set; } = new JwtTokenOptions();

		public JwtTokenOptions RefreshTokenOptions { get; set; } = new JwtTokenOptions();
    }
}
