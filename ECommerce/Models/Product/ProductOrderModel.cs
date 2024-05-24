using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Order
{
    public class ProductOrderModel
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public byte Discount { get; set; }
    }
}
