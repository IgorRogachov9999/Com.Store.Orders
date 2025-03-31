using Com.Store.Orders.Domain.Data.Enums;

namespace Com.Store.Orders.Domain.Services.Events
{
    public class OrderStatusUpdated
    {
        public Guid OrderId { get; set; }
            
        public OrderStatus Status { get; set; }
    }
}
