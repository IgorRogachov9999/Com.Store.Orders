using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Com.Store.Orders.Domain.Services.Dto;
using Com.Store.Orders.Domain.Services.Services.Contracts;

namespace Com.Store.Orders.Domain.Services.Services
{
    public class OrderStatusUpdatedHandlerService : IOrderStatusUpdatedHandlerService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderStatusUpdatedHandlerService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task HandleAsync(OrderStatusUpdatedDto orderStatusUpdated, CancellationToken ct)
        {
            switch (orderStatusUpdated.Status)
            {
                case OrderStatus.Pending:
                    throw new ArgumentException("Status can not be updated to Pending.");
                case OrderStatus.Confirmed:
                    await _orderRepository.ConfirmOrderAsync(orderStatusUpdated.OrderId, orderStatusUpdated.Timestamp, ct);
                    break;
                case OrderStatus.Shipped:
                    await _orderRepository.ShipOrderAsync(orderStatusUpdated.OrderId, orderStatusUpdated.Timestamp, ct);
                    break;
            }
        }
    }
}
