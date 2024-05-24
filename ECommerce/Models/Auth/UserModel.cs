namespace ECommerce.Models.Auth
{
    public class UserModel
    {
        public required string Id { get; set; }

        public required string Address { get; set; }


        public required string DisplayName { get; set; }


        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public List<string>? Role { get; set; }
    }
}
