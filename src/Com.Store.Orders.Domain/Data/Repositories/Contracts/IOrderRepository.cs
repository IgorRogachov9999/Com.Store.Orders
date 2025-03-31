using Com.Store.Orders.Domain.Data.Entities;
using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Data.Models.Pagination;

namespace Com.Store.Orders.Domain.Data.Repositories.Contracts
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<PageModel<Order>> GetOrdersAsync(PaginationSettingsModel paginationSettings, CancellationToken ct);

        Task<OrderStatus?> GetStatusByOrderIdAsync(Guid orderId, CancellationToken ct);

        Task CreateOrderAsync(Order order, CancellationToken ct);

        Task<bool> ConfirmOrderAsync(Guid orderId, DateTime updateDate, CancellationToken ct);

        Task<bool> ShipOrderAsync(Guid orderId, DateTime updateDate, CancellationToken ct);
    }
}
