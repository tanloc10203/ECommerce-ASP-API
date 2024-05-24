using AutoMapper;
using ECommerce.Data;
using ECommerce.Models.Order;
using ECommerce.Models.Pagination;
using ECommerce.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.OrderService
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BaseDbContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(
            BaseDbContext context,
            IMapper mapper
        )
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse> Add(OrderModel orderModel)
        {
            var order = new Order
            {
                PhoneNumber = orderModel.PhoneNumber,
                Receiver = orderModel.Receiver,
                Status = OrderStatus.New,
                ReceiverAddress = orderModel.ReceiverAddress,
                UserId = orderModel.UserId,
            };

            /* var order = _mapper.Map<Order>(orderModel);*/

            // Tạo đơn hàng
            await _context.Orders.AddAsync(order);

            // Tạo sản phẩm chi tiết đơn hàng

            await _context.SaveChangesAsync();

            var productOrderInsert = orderModel.ProductOrders.ToList().ConvertAll(x =>
            {
                return new ProductOrder
                {
                    ProductId = x.ProductId,
                    Discount = x.Discount,
                    OrderId = order.Id,
                    Price = x.Price,
                    Quantity = x.Quantity,
                };
            });

            await _context.ProductOrders.AddRangeAsync(productOrderInsert);

            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                Success = true,
                Data = orderModel,
                Message = $"Order success with order id {order.Id.ToString()}",
                StatusCode = StatusCodes.Status201Created
            };
        }

        public async Task<ApiResponse> ChangeStatus(string oId)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id.Equals(Guid.Parse(oId)));

            if (order == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Data = null,
                    Message = string.Format("Order not found with id = {0}", oId),
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            OrderStatus orderStatus = order.Status;
            var orderStatusDefault = order.Status;

            // Xét lại trạng thái
            switch (orderStatus)
            {
                case OrderStatus.New:
                    orderStatus = OrderStatus.Transported;
                    break;
                case OrderStatus.Transported:
                    orderStatus = OrderStatus.Complete;
                    break;
                case OrderStatus.Cancel:
                    orderStatus = OrderStatus.Cancel;
                    break;
                default:
                    orderStatus = OrderStatus.Complete;
                    break;
            }

            // Cập nhật trạng thái
            order.Status = orderStatus;

            _context.Orders.Update(order);

            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                Success = true,
                Data = null,
                Message = string.Format("Update status order with id = {0} success. From status = {1} to status = {2}", oId, orderStatusDefault, orderStatus)
            };
        }

        public async Task<SuccessResponse> GetAll(PaginationOrder? pagination)
        {
            var orders = _context.Orders.AsQueryable();

            #region Include Relationship
            if (!string.IsNullOrEmpty(pagination?.UserId))
            {
                orders = orders.Where(t => t.User!.Id.Equals(pagination.UserId))
                    .Include("ProductOrders.Product")
                    .Include("ProductOrders.Product.Category");
            }
            else
            {
                orders = orders
                    .Include("User")
                    .Include("ProductOrders.Product")
                    .Include("ProductOrders.Product.Category");
            }
            #endregion

            #region Sort
            if (!string.IsNullOrEmpty(pagination?.SortBy))
            {
                /*orders = orders.OrderByDescending(p => p.User);*/
                switch (pagination?.SortBy)
                {
                    case "status_asc":
                        orders = orders.OrderBy(p => p.Status);
                        break;
                    case "status_desc":
                        orders = orders.OrderByDescending(p => p.Status);
                        break;
                }
            }
            else
            {
                orders = orders.OrderBy(p => p.CreatedAt);
            }
            #endregion


            // Giá trị đã set mặc định trong class PaginationBase
            int page = (int)pagination?.Page!;
            int pageSize = (int)pagination?.PageSize!;

            var result = await PaginatedList<Order>.Create(orders, page, pageSize);

            return new SuccessResponse
            {
                Data = _mapper.Map<List<OrderViewModel>>(result),
                Message = "Get all order success",
                Paginations = new PaginationResponse
                {
                    PageIndex = result.PageIndex,
                    PageSize = pageSize,
                    TotalPage = result.TotalPage
                },
                Success = true
            };
        }

        public async Task<ApiResponse> GetById(string oId)
        {
            var order = await _context.Orders
                .Include("User")
                .Include("ProductOrders.Product")
                .Include("ProductOrders.Product.Category")
                .SingleOrDefaultAsync(o => o.Id.Equals(Guid.Parse(oId)));

            if (order == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Data = null,
                    Message = string.Format("Order not found with id = {0}", oId),
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            return new ApiResponse
            {
                Success = true,
                Data = _mapper.Map<OrderViewModel>(order),
                Message = string.Format("Get order by id = {0} success", oId)
            };
        }
    }
}
