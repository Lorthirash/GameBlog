using System.ComponentModel.DataAnnotations;

namespace Backend.Models.EmailSettings
{
    public class EmailDetails
    {
        [Required(ErrorMessage = "Subject is required"), MaxLength(40, ErrorMessage = "Subject should be less than 40 character")]
        public string EmailSubject { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email body section is required")]
        public string EmailBody { get; set; } = string.Empty;
    }
}
