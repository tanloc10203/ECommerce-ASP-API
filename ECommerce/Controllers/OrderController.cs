using ECommerce.Models;
using ECommerce.Models.Order;
using ECommerce.Models.Pagination;
using ECommerce.Models.Response;
using ECommerce.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderModel orderModel)
        {
            try
            {
                var saved = await _orderRepository.Add(orderModel);

                int statusCode = saved?.StatusCode ?? 200;

                return StatusCode(statusCode, saved);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = e.Message,
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationOrder? pagination)
        {
            try
            {
                var response = await _orderRepository.GetAll(pagination);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = e.Message,
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPatch("ChangeStatus/{orderId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> ChangeStatus([FromRoute] string orderId)
        {
            try
            {
                var response = await _orderRepository.ChangeStatus(orderId);

                var statusCode = response.StatusCode ?? 200;

                return StatusCode(statusCode, response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = e.Message,
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpGet("{orderId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetOrderById([FromRoute] string orderId)
        {
            try
            {
                var response = await _orderRepository.GetById(orderId);

                var statusCode = response.StatusCode ?? 200;

                return StatusCode(statusCode, response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = e.Message,
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
