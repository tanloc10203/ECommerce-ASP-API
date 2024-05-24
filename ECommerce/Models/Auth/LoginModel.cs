using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Auth
{
    public class LoginModel
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Password { get; set; }
    }
}
