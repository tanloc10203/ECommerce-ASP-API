using Microsoft.AspNetCore.Identity;

namespace ECommerce.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }
}
