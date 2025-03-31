using AutoFixture.AutoMoq;
using AutoFixture;
using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Com.Store.Orders.Domain.Services.Services;
using Com.Store.Orders.Domain.Tests.Customizations;
using Moq;
using Com.Store.Orders.Domain.Services.Services.Contracts;
using Com.Store.Orders.Domain.Services.Exceptions;
using Com.Store.Orders.Domain.Data.Entities;
using FluentAssertions;
using Com.Store.Orders.Domain.Data.Models.Pagination;
using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Services.Dto;
using Com.Store.Orders.Domain.Services.Events;

namespace Com.Store.Orders.Domain.Tests.Services.Services
{
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _orderRepository;
        private Mock<IItemRepository> _itemRepository;
        private Mock<IEventProducingService> _eventProducingService;
        private OrderService _service;
        private IFixture _fixture;

        public OrderServiceTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new AutoMapperCustomization());

            _orderRepository = _fixture.Freeze<Mock<IOrderRepository>>();
            _itemRepository = _fixture.Freeze<Mock<IItemRepository>>();
            _eventProducingService = _fixture.Freeze<Mock<IEventProducingService>>();
            _service = _fixture.Create<OrderService>();
        }

        [Fact]
        public async Task GetByIdAsync_Should_ThrowDomainEntityNotFoundException_When_NoOrder()
        {
            // Arrange
            var ct = CancellationToken.None;
            var id = _fixture.Create<Guid>();
            _orderRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync((Order)null);

            // Act & Assert
            await Assert.ThrowsAsync<DomainEntityNotFoundException>(
                async () => await _service.GetByIdAsync(id, ct));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnOrder()
        {
            // Arrange
            var ct = CancellationToken.None;
            var id = _fixture.Create<Guid>();
            var order = _fixture.Build<Order>()
                .With(p => p.Items,
                    _fixture.Build<OrderItem>()
                        .Without(p => p.Order)
                        .Without(p => p.Item)
                        .CreateMany().ToList())
                .Create();
            _orderRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(order);

            // Act
            var result = await _service.GetByIdAsync(id, ct);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(order.Id);
            result.Items.Count().Should().Be(order.Items.Count());
        }

        [Fact]
        public async Task GetOrdersAsync_Should_ReturnOrdersPage()
        {
            // Arrange
            var ct = CancellationToken.None;
            var paginationSettings = _fixture.Create<PaginationSettingsModel>();
            var orders = _fixture.Build<PageModel<Order>>()
                .With(p => p.Items,
                    _fixture.Build<Order>()
                        .With(p => p.Items,
                            _fixture.Build<OrderItem>()
                                .Without(p => p.Order)
                                .Without(p => p.Item)
                                .CreateMany().ToList())
                        .CreateMany().ToList())
                .Create();
            _orderRepository
                .Setup(x => x.GetOrdersAsync(It.IsAny<PaginationSettingsModel>(), ct))
                .ReturnsAsync(orders);

            // Act
            var result = await _service.GetOrdersAsync(paginationSettings, ct);

            // Assert
            result.Should().NotBeNull();
            result.Items.Count().Should().Be(orders.Items.Count());
        }

        [Fact]
        public async Task GetStatusByOrderIdAsync_Should_ThrowDomainEntityNotFoundException_When_NoOrder()
        {
            // Arrange
            var ct = CancellationToken.None;
            var id = _fixture.Create<Guid>();
            _orderRepository
                .Setup(x => x.GetStatusByOrderIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync((OrderStatus?)null);

            // Act & Assert
            await Assert.ThrowsAsync<DomainEntityNotFoundException>(
                async () => await _service.GetStatusByOrderIdAsync(id, ct));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnStatus()
        {
            // Arrange
            var ct = CancellationToken.None;
            var id = _fixture.Create<Guid>();
            var status = _fixture.Create<OrderStatus>();
            _orderRepository
                .Setup(x => x.GetStatusByOrderIdAsync(It.IsAny<Guid>(), ct))
                .ReturnsAsync(status);

            // Act
            var result = await _service.GetStatusByOrderIdAsync(id, ct);

            // Assert
            result.Should().Be(status);
        }

        [Fact]
        public async Task CreateOrderAsync_Should_ThrowDomainEntityNotFoundException_When_NoItem()
        {
            // Arrange
            var ct = CancellationToken.None;
            var createOrder = _fixture.Create<CreateOrderDto>();
            _itemRepository
                .Setup(x => x.ContainsAsync(It.IsAny<Guid[]>(), ct))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<DomainEntityNotFoundException>(
                async () => await _service.CreateOrderAsync(createOrder, ct));
        }

        [Fact]
        public async Task CreateOrderAsync_Should_CreateOrder()
        {
            // Arrange
            var ct = CancellationToken.None;
            var createOrder = _fixture.Create<CreateOrderDto>();
            _itemRepository
                .Setup(x => x.ContainsAsync(It.IsAny<Guid[]>(), ct))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateOrderAsync(createOrder, ct);

            // Assert
            _orderRepository.Verify(x => x.CreateOrderAsync(It.IsAny<Order>(), ct), Times.Once());
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_Should_UpdateStatus()
        {
            // Arrange
            var ct = CancellationToken.None;
            var orderId = _fixture.Create<Guid>();
            var status = _fixture.Create<OrderStatus>();

            // Act
            var result = await _service.UpdateOrderStatusAsync(orderId, status, ct);

            // Assert
            _eventProducingService.Verify(x => x.ProduceAsync(It.IsAny<OrderStatusUpdated>(), ct), Times.Once());
        }
    }
}
