using ECommerce.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Data
{
    public enum OrderStatus
    {
        New = 0,
        Transported = 1,
        Complete = 2,
        Cancel = -1
    }

    public class Order
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]

        public ApplicationUser? User { get; set; }

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
        public ICollection<ProductOrder>? ProductOrders { get; set; }

        public Order()
        {
            // Vì tạo đơn hàng chưa có productOrder nên khởi tạo rỗng
            ProductOrders = new List<ProductOrder>();
        }
    }
}
