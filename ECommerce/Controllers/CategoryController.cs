using ECommerce.Models;
using ECommerce.Models.Category;
using ECommerce.Models.Pagination;
using ECommerce.Models.Response;
using ECommerce.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        // Khởi tạo context database
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        /* [Authorize(Roles = UserRoles.AdminOrCustomer)]*/
        public async Task<IActionResult> GetAll([FromQuery] PaginationBase? pagination)
        {
            try
            {
                var response = await _categoryRepository.GetAll(pagination);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = e.Message,
                    Success = false
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var categoryDao = await _categoryRepository.GetById(id);

                if (categoryDao == null) return NotFound(new ApiResponse
                {
                    Data = null,
                    Message = "Not found category with id = " + id,
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound
                });

                return Ok(new ApiResponse
                {
                    Data = categoryDao,
                    Message = "Get category a success",
                    Success = true
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = e.Message,
                    Success = false
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)] // Cần đăng nhập mới thực hiện được
        public async Task<IActionResult> Create(CategoryModel categoryDto)
        {
            try
            {
                var saved = await _categoryRepository.Add(categoryDto);

                return StatusCode(StatusCodes.Status201Created, new ApiResponse
                {
                    Data = saved,
                    Message = "Create a category success",
                    Success = true,
                    StatusCode = StatusCodes.Status201Created
                });
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, CategoryViewModel categoryDto)
        {
            try
            {
                var updated = await _categoryRepository.Update(id, categoryDto);

                if (!updated) return NotFound(new ApiResponse
                {
                    Data = null,
                    Message = "Not found Category",
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound
                });

                return Ok(new ApiResponse { Data = updated, Message = "Updated category success", Success = true });
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

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                var deleted = await _categoryRepository.Delete(id);

                if (!deleted) return NotFound(new ApiResponse
                {
                    Data = null,
                    Message = "Not found Category",
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound
                });

                return Ok(new ApiResponse
                {
                    Data = deleted,
                    Message = "Deleted a category success",
                    Success = true,
                });
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
