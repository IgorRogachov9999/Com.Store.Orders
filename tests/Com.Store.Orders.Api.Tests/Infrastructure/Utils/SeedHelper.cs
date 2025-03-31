using AutoFixture;
using Com.Store.Orders.Domain.Data;
using Com.Store.Orders.Domain.Data.Entities;
using Com.Store.Orders.Domain.Data.Enums;

namespace Com.Store.Orders.Api.Tests.Infrastructure.Utils
{
    public static class SeedHelper
    {
        public static Guid CreateItem(OrdersDbContext dbContext, IFixture fixture, int count = 10, decimal price = 99.99m)
        {
            var item = fixture.Build<Item>()
                .With(p => p.AvailableCount, count)
                .With(p => p.Price, price)
                .With(p => p.IsDeleted, false)
                .Without(p => p.Items)
                .Create();
            dbContext.Items.Add(item);
            dbContext.SaveChanges();
            return item.Id;
        }

        public static Guid CreateOrderItem(OrdersDbContext dbContext, IFixture fixture, Guid orderId, Guid itemId, int quantity = 1)
        {
            var orderItems = fixture.Build<OrderItem>()
                .With(p => p.OrderId, orderId)
                .With(p => p.ItemId, itemId)
                .With(p => p.IsDeleted, false)
                .With(p => p.Quantity, quantity)
                .Without(p => p.Item)
                .Without(p => p.Order)
                .Create();
            dbContext.OrderItem.Add(orderItems);
            dbContext.SaveChanges();
            return orderItems.Id;
        }

        public static Guid CreateOrder(OrdersDbContext dbContext, IFixture fixture, OrderStatus orderStatus = OrderStatus.Pending)
        {
            var order = fixture.Build<Order>()
                .With(p => p.Status, orderStatus)
                .With(p => p.IsDeleted, false)
                .Without(p => p.Items)
                .Create();
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
            return order.Id;
        }

        public static Guid CreateOrderWithItem(OrdersDbContext dbContext, IFixture fixture, OrderStatus orderStatus = OrderStatus.Pending)
        {
            var itemId = CreateItem(dbContext, fixture);
            var orderId = CreateOrder(dbContext, fixture, orderStatus);
            CreateOrderItem(dbContext, fixture, orderId, itemId);
            return orderId;
        }
    }
}
