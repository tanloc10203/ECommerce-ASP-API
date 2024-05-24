using ECommerce.Models.Pagination;

namespace ECommerce.Models.Response
{
    public class SuccessResponse : ApiResponse
    {
        public PaginationResponse? Paginations { get; set; }
    }
}
