using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class UserDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        [Required]
        public IList<string> Roles { get; set; }
    }
}
