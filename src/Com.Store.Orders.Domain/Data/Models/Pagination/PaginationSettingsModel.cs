using Com.Store.Orders.Domain.Data.Models.Pagination.Enums;

namespace Com.Store.Orders.Domain.Data.Models.Pagination
{
    public class PaginationSettingsModel
    {
        public PaginationSortingOrder? SortingOrder { get; set; }

        public int PageSize { get; set; } = 20;

        public int PageNumber { get; set; } = 1;
    }
}
