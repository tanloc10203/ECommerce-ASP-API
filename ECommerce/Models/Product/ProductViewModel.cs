namespace ECommerce.Models.Product
{
    public class ProductViewModel : ProductModel
    {
        public Guid Id { get; set; }

        public required string Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
