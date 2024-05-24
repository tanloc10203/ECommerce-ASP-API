namespace ECommerce.Data
{
    public class ProductOrder
    {
        // Bảng này có 2 khóa chính do phụ thuộc hàm vào bảng order và product
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public byte Discount { get; set; }

        // relationship
        public virtual Order? Order { get; set; }

        public virtual Product? Product { get; set; }
    }
}
