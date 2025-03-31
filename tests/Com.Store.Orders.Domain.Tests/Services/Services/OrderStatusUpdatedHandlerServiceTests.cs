using AutoFixture.AutoMoq;
using AutoFixture;
using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Com.Store.Orders.Domain.Services.Services;
using Com.Store.Orders.Domain.Tests.Customizations;
using Moq;
using Com.Store.Orders.Domain.Services.Dto;
using Com.Store.Orders.Domain.Data.Enums;

namespace Com.Store.Orders.Domain.Tests.Services.Services
{
    public class OrderStatusUpdatedHandlerServiceTests
    {
        private Mock<IOrderRepository> _orderRepository;
        private OrderStatusUpdatedHandlerService _service;
        private IFixture _fixture;

        public OrderStatusUpdatedHandlerServiceTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new AutoMapperCustomization());

            _orderRepository = _fixture.Freeze<Mock<IOrderRepository>>();
            _service = _fixture.Create<OrderStatusUpdatedHandlerService>();
        }

        [Fact]
        public async Task HandleAsync_Should_ThrowArgumentException_When_StatusIsPending()
        {
            // Arrange
            var ct = CancellationToken.None;
            var orderStatusUpdated = _fixture.Build<OrderStatusUpdatedDto>()
                .With(p => p.Status, OrderStatus.Pending)
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await _service.HandleAsync(orderStatusUpdated, ct));
        }

        [Fact]
        public async Task HandleAsync_Should_ConfirmOrder_When_StatusIsConfirmed()
        {
            // Arrange
            var ct = CancellationToken.None;
            var orderStatusUpdated = _fixture.Build<OrderStatusUpdatedDto>()
                .With(p => p.Status, OrderStatus.Confirmed)
                .Create();

            // Act
            await _service.HandleAsync(orderStatusUpdated, ct);

            // Assert
            _orderRepository.Verify(x => x.ConfirmOrderAsync(orderStatusUpdated.OrderId, orderStatusUpdated.Timestamp, ct), Times.Once());
            _orderRepository.Verify(x => x.ShipOrderAsync(orderStatusUpdated.OrderId, orderStatusUpdated.Timestamp, ct), Times.Never());
        }

        [Fact]
        public async Task HandleAsync_Should_ShipOrder_When_StatusIsShiped()
        {
            // Arrange
            var ct = CancellationToken.None;
            var orderStatusUpdated = _fixture.Build<OrderStatusUpdatedDto>()
                .With(p => p.Status, OrderStatus.Shipped)
                .Create();

            // Act
            await _service.HandleAsync(orderStatusUpdated, ct);

            // Assert
            _orderRepository.Verify(x => x.ConfirmOrderAsync(orderStatusUpdated.OrderId, orderStatusUpdated.Timestamp, ct), Times.Never());
            _orderRepository.Verify(x => x.ShipOrderAsync(orderStatusUpdated.OrderId, orderStatusUpdated.Timestamp, ct), Times.Once());
        }
    }
}
