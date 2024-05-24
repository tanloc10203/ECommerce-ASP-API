namespace ECommerce.Models.Product
{
    public class ProductOrderViewModel
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public byte Discount { get; set; }

        public ProductViewModel? Product { get; set; }
    }
}
