using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Category
{
    public class CategoryModel
    {
        [Required]
        public required string Name { get; set; }
    }
}
