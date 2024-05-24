using AutoMapper;
using ECommerce.Data;
using ECommerce.Models.Pagination;
using ECommerce.Models.Product;
using ECommerce.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.ProductService
{
    public class ProductRepository : IProductRepository
    {
        private readonly BaseDbContext _context;
        private readonly IMapper _mapper;

        public ProductRepository(BaseDbContext baseDbContext, IMapper mapper)
        {
            _context = baseDbContext;
            _mapper = mapper;
        }

        async Task<SuccessResponse> IProductRepository.GetAll(PaginationModel pagination)
        {
            var products = _context.Products.Include(t => t.Category).AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                products = products.Where(p => p.Name.Contains(pagination.Search));
            }

            if (pagination.CategoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId.Equals(pagination.CategoryId));
            }

            if (pagination.IsPopulate.HasValue && pagination.IsPopulate.Equals(true))
            {
                products = products.Where((t) => t.IsPopulate.Equals(pagination.IsPopulate));
            }
            #endregion

            #region Sorting


            if (!string.IsNullOrEmpty(pagination.SortBy))
            {
                switch (pagination.SortBy)
                {
                    case "name_desc":
                        products = products.OrderByDescending(p => p.Name);
                        break;

                    case "price_asc":
                        products = products.OrderBy(p => p.Price);
                        break;

                    case "created_asc":
                        products = products.OrderBy(p => p.CreatedAt);
                        break;

                    case "created_desc":
                        products = products.OrderByDescending(p => p.CreatedAt);
                        break;

                    case "price_desc":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                }
            }
            #endregion

            #region Paging

            // Giá trị đã set mặc định trong class PaginationBase
            int page = (int)pagination?.Page!;
            int pageSize = (int)pagination?.PageSize!;

            #endregion

            var result = await PaginatedList<Product>.Create(products, page, pageSize);

            return new SuccessResponse
            {
                Data = _mapper.Map<List<ProductViewModel>>(result),
                Message = "Get all product success",
                Paginations = new PaginationResponse
                {
                    PageIndex = result.PageIndex,
                    PageSize = pageSize,
                    TotalPage = result.TotalPage
                },
                Success = true
            };
        }

        async Task<ProductViewModel?> IProductRepository.GetById(string id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(t => t.Id.Equals(Guid.Parse(id)));

            if (product == null) return null;

            return _mapper.Map<ProductViewModel>(product);
        }

        async Task<ProductViewModel> IProductRepository.Add(ProductModel pDto)
        {
            var product = _mapper.Map<Product>(pDto);

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return _mapper.Map<ProductViewModel>(product);
        }

        async Task<bool> IProductRepository.Update(string id, ProductModel pDto)
        {
            var product = await _context.Products.FindAsync(Guid.Parse(id));

            if (product == null) return false;

            product.Description = pDto.Description;
            product.Price = pDto.Price;
            product.CategoryId = pDto.CategoryId;
            product.Discount = pDto.Discount;
            product.IsPopulate = pDto.IsPopulate;

            if (!string.IsNullOrEmpty(pDto.Image))
            {
                product.Image = pDto.Image;
            }

            _context.Products.Update(product);

            await _context.SaveChangesAsync();

            return true;
        }

        async Task<bool> IProductRepository.Delete(string id)
        {
            var product = await _context.Products.FindAsync(Guid.Parse(id));

            if (product == null) return false;

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
