using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Order
{
    public class OrderModel
    {
        public string? UserId { get; set; }

        // Người nhận hàng
        [Required]
        public string Receiver { get; set; } = string.Empty;

        // Địa chỉ người nhận
        [Required]
        public string ReceiverAddress { get; set; } = string.Empty;

        // Số điện thoại
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        // Chi tiết các sản phẩm order
        [Required]
        public ICollection<ProductOrderModel> ProductOrders { get; set; } = new List<ProductOrderModel>();
    }
}
