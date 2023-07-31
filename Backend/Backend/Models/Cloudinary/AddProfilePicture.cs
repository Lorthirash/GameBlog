using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Cloudinary
{
    public class AddProfilePicture
    {
        [Required(ErrorMessage = "File is required")]
        public IFormFile? Image { get; set; }
    }
}
