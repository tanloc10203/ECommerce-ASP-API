using AutoMapper;
using ECommerce.Data;
using ECommerce.Models.Category;
using ECommerce.Models.Pagination;
using ECommerce.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.CategoryService
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BaseDbContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(BaseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CategoryViewModel> Add(CategoryModel categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(category => category.Id.Equals(id));

            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<SuccessResponse> GetAll(PaginationBase? pagination)
        {
            var categories = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(pagination?.Search))
            {
                categories = categories.Where(p => p.Name.Contains(pagination.Search));
            }

            // Giá trị đã set mặc định trong class PaginationBase
            int page = (int)pagination?.Page!;
            int pageSize = (int)pagination?.PageSize!;

            var result = await PaginatedList<Category>.Create(categories, page, pageSize);

            return new SuccessResponse
            {
                Data = _mapper.Map<List<CategoryViewModel>>(result),
                Message = "Get all category success",
                Paginations = new PaginationResponse
                {
                    PageIndex = result.PageIndex,
                    PageSize = pageSize,
                    TotalPage = result.TotalPage
                },
                Success = true
            };
        }

        public async Task<CategoryViewModel?> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) return null;

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<bool> Update(int id, CategoryViewModel categoryDto)
        {
            if (!id.Equals(categoryDto.Id)) return false;

            var updated = _mapper.Map<Category>(categoryDto);

            _context.Categories.Update(updated);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
