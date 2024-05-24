using ECommerce.Models;
using ECommerce.Models.Pagination;
using ECommerce.Models.Product;
using ECommerce.Models.Response;
using ECommerce.Services.FileService;
using ECommerce.Services.ProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;


        public ProductController(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] PaginationModel pagination)
        {
            try
            {
                var result = await _productRepository.GetAll(pagination);

                return Ok(result);
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
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            try
            {
                // Truy vấn database theo id
                var productDao = await _productRepository.GetById(id);

                if (productDao == null) return NotFound(new ApiResponse
                {
                    Data = null,
                    Message = "Not found product with id = " + id,
                    Success = false
                });

                return Ok(new ApiResponse
                {
                    Data = productDao,
                    Message = string.Format("Get Product by id = {0} Success", id),
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
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Create([FromForm] ProductModel model)
        {
            try
            {
                if (model.ImageFile != null)
                {
                    var fileResult = _fileService.SaveImage(model.ImageFile);

                    // Nếu bị lỗi sẽ return
                    if (fileResult.Item1.Equals(0))
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse
                        {
                            Data = null,
                            Message = fileResult.Item2,
                            Success = false
                        });
                    }

                    model.Image = fileResult.Item2;
                }


                var saved = await _productRepository.Add(model);

                return StatusCode(StatusCodes.Status201Created, new ApiResponse
                {
                    Data = saved,
                    Message = "Created product success",
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

        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] ProductModel updateDto)
        {
            try
            {
                if (updateDto.ImageFile != null)
                {
                    var fileResult = _fileService.SaveImage(updateDto.ImageFile);

                    // Nếu bị lỗi sẽ return
                    if (fileResult.Item1.Equals(0))
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse
                        {
                            Data = null,
                            Message = fileResult.Item2,
                            Success = false
                        });
                    }

                    updateDto.Image = fileResult.Item2;
                }


                var updated = await _productRepository.Update(id, updateDto);

                if (!updated) return NotFound(new ApiResponse
                {
                    Data = null,
                    Message = "Not found product with id = " + id,
                    Success = false,
                    StatusCode = 404
                });

                return Ok(new ApiResponse { Data = updated, Message = "Updated product success", Success = true });
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

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Remove([FromRoute] string id)
        {
            try
            {
                var deleted = await _productRepository.Delete(id);

                if (!deleted) return NotFound(new ApiResponse
                {
                    Data = null,
                    Message = "Not found product with id = " + id,
                    Success = false
                });

                return Ok(new ApiResponse
                {
                    Data = deleted,
                    Message = string.Format("Deleted product with id = {0} success", id),
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
    }
}
