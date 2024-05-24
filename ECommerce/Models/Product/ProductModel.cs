using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models.Product
{
    public class ProductModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Description { get; set; }

        [Required]
        public required byte Discount { get; set; }

        public bool IsPopulate { get; set; } = false;

        [Required]
        public required int CategoryId { get; set; }

        public string? Image { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
