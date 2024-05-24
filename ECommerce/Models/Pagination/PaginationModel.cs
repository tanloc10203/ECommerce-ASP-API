namespace ECommerce.Models.Pagination
{
    public class PaginationBase
    {
        public string? Search { get; set; }

        public string? SortBy { get; set; }

        public int? Page { get; set; } = 1;

        public int? PageSize { get; set; } = 5;
    }

    public class PaginationOrder : PaginationBase
    {
        public string? UserId { get; set; } = string.Empty;
    }

    public class PaginationModel : PaginationBase
    {
        public bool? IsPopulate { get; set; } = false;

        public int? CategoryId { get; set; }
    }

    public class PaginationResponse
    {
        public int TotalPage { get; set; } = 10;

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
