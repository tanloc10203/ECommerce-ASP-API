using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Data
{
    [Table("Product")]
    public class Product
    {
        [Key] // Khóa chính
        public Guid Id { get; set; }

        [Required] // Bắt buộc
        [MaxLength(100)] // Tên sản phẩm nhiều nhất 100 kí tự
        public string Name { get; set; }

        // Được phép null or có thể có hoặc không
        public string? Description { get; set; }

        [Range(0, double.MaxValue)] // Đơn giá có giá trị từ 0 -> Max
        public double Price { get; set; }

        public byte Discount { get; set; }

        public string? Image { get; set; }

        public bool IsPopulate { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Khóa ngoại 
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]

        public Category? Category { get; set; }

        // Chi tiết đặt hàng
        public ICollection<ProductOrder>? ProductOrders { get; set; }

        public Product()
        {
            // Vì tạo đơn hàng chưa có productOrder nên khởi tạo rỗng
            // HashSet tương đương List
            ProductOrders = new HashSet<ProductOrder>();
        }
    }
}
