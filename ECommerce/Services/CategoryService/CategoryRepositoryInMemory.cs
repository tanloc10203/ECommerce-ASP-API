using ECommerce.Models.Category;

namespace ECommerce.Services.CategoryService
{
    public class CategoryRepositoryInMemory
    {
        static List<CategoryViewModel> categories = new List<CategoryViewModel>
        {
            new CategoryViewModel { Id = 1, Name = "Tivi"},
            new CategoryViewModel { Id = 2, Name = "Laptop"},
            new CategoryViewModel { Id = 3, Name = "Máy tính bảng"},
            new CategoryViewModel { Id = 4, Name = "Iphone"},
            new CategoryViewModel { Id = 5, Name = "Samsung"},
        };

        public CategoryViewModel Add(CategoryModel categoryDto)
        {
            var category = new CategoryViewModel
            {
                Id = categories.Max(c => c.Id) + 1,
                Name = categoryDto.Name,
            };

            categories.Add(category);

            return category;
        }

        public bool Delete(int id)
        {
            var category = categories.SingleOrDefault(c => c.Id == id);

            if (category == null) return false;

            categories.Remove(category);

            return true;
        }

        public List<CategoryViewModel> GetAll()
        {
            return categories;
        }

        public CategoryViewModel? GetById(int id)
        {
            return categories.SingleOrDefault(c => c.Id == id);
        }

        public bool Update(int id, CategoryModel categoryDto)
        {
            var category = categories.SingleOrDefault(c => c.Id == id);

            if (category == null) return false;

            category.Name = categoryDto.Name;

            return true;
        }
    }
}
