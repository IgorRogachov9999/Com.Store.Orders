using AutoMapper;
using Com.Store.Orders.Domain.Data.Entities;
using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Services.Dto;

namespace Com.Store.Orders.Domain.Services.Mapper.Converters
{
    public class CreateOrderDtoToOrderConverter : ITypeConverter<CreateOrderDto, Order>
    {
        public Order Convert(CreateOrderDto source, Order destination, ResolutionContext context)
        {
            var orderId = Guid.NewGuid();
            destination = new Order
            {
                Id = orderId,
                Name = source.Name,
                Email = source.Email,
                Phone = source.Phone,
                Address = source.Address,
                Notes = source.Notes,
                Status = OrderStatus.Pending,
                Items = source.Items.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    ItemId = item.Id,
                    Quantity = item.Quantity
                }).ToList()
            };

            return destination;
        }
    }
}