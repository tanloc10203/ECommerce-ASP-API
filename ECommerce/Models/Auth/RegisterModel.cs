using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Auth
{
    public class RegisterModel
    {
        [Required]
        public required string Address { get; set; }

        [Required]
        public required string DisplayName { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, MaxLength(255)]
        public required string Password { get; set; }

        [Required, MaxLength(255)]
        public required string ConfirmPassword { get; set; }

        [Required, Phone]
        public required string PhoneNumber { get; set; }
    }
}
