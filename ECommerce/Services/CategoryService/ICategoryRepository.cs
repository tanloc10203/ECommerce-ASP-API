using ECommerce.Models.Category;
using ECommerce.Models.Pagination;
using ECommerce.Models.Response;

namespace ECommerce.Services.CategoryService
{
    public interface ICategoryRepository
    {
        Task<SuccessResponse> GetAll(PaginationBase? pagination);

        Task<CategoryViewModel?> GetById(int id);

        Task<CategoryViewModel> Add(CategoryModel model);

        Task<bool> Update(int id, CategoryViewModel model);

        Task<bool> Delete(int id);
    }
}
