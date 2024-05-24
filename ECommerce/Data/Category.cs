using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Data
{
    [Table("Category")]
    public class Category
    {
        [Key] // Khóa chính
        public int Id { get; set; }

        [Required] // Bắt buộc
        [MaxLength(100)] // Kí tự tối đa
        public string Name { get; set; }

        // 1 - N (1 Danh mục có nhiều hàng hóa)
        public virtual ICollection<Product>? Products { get; set; }
    }
}
