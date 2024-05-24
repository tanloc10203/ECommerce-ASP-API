using ECommerce.Models.Order;
using ECommerce.Models.Pagination;
using ECommerce.Models.Response;

namespace ECommerce.Services.OrderService
{
    public interface IOrderRepository
    {
        public Task<ApiResponse> Add(OrderModel orderModel);

        public Task<SuccessResponse> GetAll(PaginationOrder? pagination);

        public Task<ApiResponse> ChangeStatus(string oId);

        public Task<ApiResponse> GetById(string oId);
    }
}
