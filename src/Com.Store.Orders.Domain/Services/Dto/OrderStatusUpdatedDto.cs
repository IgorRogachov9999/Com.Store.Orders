using Com.Store.Orders.Domain.Data.Enums;

namespace Com.Store.Orders.Domain.Services.Dto
{
    public class OrderStatusUpdatedDto
    {
        public Guid OrderId { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
