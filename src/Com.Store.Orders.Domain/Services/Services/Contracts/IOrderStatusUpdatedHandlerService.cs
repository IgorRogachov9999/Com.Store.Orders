using Com.Store.Orders.Domain.Services.Dto;

namespace Com.Store.Orders.Domain.Services.Services.Contracts
{
    public interface IOrderStatusUpdatedHandlerService
    {
        public Task HandleAsync(OrderStatusUpdatedDto orderStatusUpdated, CancellationToken ct);
    }
}
