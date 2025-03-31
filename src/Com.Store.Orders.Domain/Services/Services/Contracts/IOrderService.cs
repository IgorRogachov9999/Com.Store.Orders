using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Data.Models.Pagination;
using Com.Store.Orders.Domain.Services.Dto;

namespace Com.Store.Orders.Domain.Services.Services.Contracts
{
    public interface IOrderService
    {
        Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct);

        Task<PageModel<OrderDto>> GetOrdersAsync(PaginationSettingsModel paginationSettings, CancellationToken ct);

        Task<OrderStatus> GetStatusByOrderIdAsync(Guid id, CancellationToken ct);

        Task<Guid> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken ct);

        Task<Guid> UpdateOrderStatusAsync(Guid orderId, OrderStatus orderStatus, CancellationToken ct);
    }
}
