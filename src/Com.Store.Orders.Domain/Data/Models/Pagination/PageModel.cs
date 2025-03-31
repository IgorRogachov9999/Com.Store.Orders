namespace Com.Store.Orders.Domain.Data.Models.Pagination
{
    public class PageModel<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
