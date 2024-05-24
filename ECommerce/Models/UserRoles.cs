namespace ECommerce.Models
{
    public class UserRoles
    {
        public const string Admin = "admin";
        public const string Customer = "customer";
        public const string AdminOrCustomer = Admin + "," + Customer;
    }
}
