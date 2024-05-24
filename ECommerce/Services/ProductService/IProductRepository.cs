using ECommerce.Models.Pagination;
using ECommerce.Models.Product;
using ECommerce.Models.Response;

namespace ECommerce.Services.ProductService
{
    public interface IProductRepository
    {
        Task<SuccessResponse> GetAll(PaginationModel pagination);

        Task<ProductViewModel?> GetById(string id);

        Task<ProductViewModel> Add(ProductModel pDto);

        Task<bool> Update(string id, ProductModel pDto);

        Task<bool> Delete(string id);
    }
}
