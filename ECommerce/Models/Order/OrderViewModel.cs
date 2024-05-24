using ECommerce.Data;
using ECommerce.Models.Auth;
using ECommerce.Models.Product;

namespace ECommerce.Models.Order
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        public UserModel? User { get; set; }

        // Ngày đặt
        public DateTime CreatedAt { get; set; }

        // Ngày giao
        public DateTime? DeliveryDate { get; set; }

        public OrderStatus Status { get; set; }

        // Người nhận hàng
        public string Receiver { get; set; }

        // Địa chỉ người nhận
        public string ReceiverAddress { get; set; }

        // Số điện thoại
        public string PhoneNumber { get; set; }

        // Chi tiết các sản phẩm order
        public ICollection<ProductOrderViewModel>? ProductOrders { get; set; }
    }
}
