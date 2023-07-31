using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required!")]
        [MinLength(3, ErrorMessage = "Username should be at least 3 characters long!")]
        [MaxLength(50, ErrorMessage = "Username should be max 50 characters long!")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        [RegularExpression(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$", ErrorMessage = "Invalid email address!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(8, ErrorMessage = "Password should be at least 8 characters long!")]
        [MaxLength(40, ErrorMessage = "Password should be max 40 characters long!")]
        [RegularExpression(@"^[a-zA-Z0-9]{8,40}", ErrorMessage = "Password can only contain letters and numbers")]
        public string Password { get; set; } = string.Empty;
        public bool IsSubscribed { get; set; } = false;
    }
}
