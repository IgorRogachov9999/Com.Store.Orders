using Com.Store.Orders.Domain.Data.Entities;
using Com.Store.Orders.Domain.Data.Models.Pagination.Enums;

namespace Com.Store.Orders.Domain.Data.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Order<T>(this IQueryable<T> query, PaginationSortingOrder sortingOrder) where T : AuditableEntityBase
        {
            return sortingOrder == PaginationSortingOrder.Ascending
                    ? query.OrderBy(x => x.UpdatedAt.GetValueOrDefault(x.CreatedAt))
                    : query.OrderByDescending(x => x.UpdatedAt.GetValueOrDefault(x.CreatedAt));
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageSize, int pageNumber)
        {
            return query
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize);
        }
    }
}
