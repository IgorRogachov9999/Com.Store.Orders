using AutoFixture;
using Com.Store.Orders.Api.Tests.Infrastructure;
using Com.Store.Orders.Api.Tests.Infrastructure.Utils;
using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Services.Dto;
using FluentAssertions;
using System.Net;

namespace Com.Store.Orders.Api.Tests.Controllers
{
    public class OrderControllerTests : TestSuteBase
    {
        public OrderControllerTests(OrdersWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetOrderStatusAsync_Should_ReturnEntityNotFoundException_When_NoOrder()
        {
            // Arrange
            var orderId = _fixture.Create<Guid>();

            // Act
            var response = await _client.GetAsync<OrderStatus>($"/api/v1/orders/{orderId}/status");

            // Assert
            response.EnsureStatusCode((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetOrderStatusAsync_Should_OrderStatus()
        {
            // Arrange
            var orderStatus = _fixture.Create<OrderStatus>();
            var orderId = SeedHelper.CreateOrderWithItem(_dbContext, _fixture, orderStatus);

            // Act
            var response = await _client.GetAsync<OrderStatus>($"/api/v1/orders/{orderId}/status");

            // Assert
            response.EnsureStatusCode((int)HttpStatusCode.OK);
            response.Body.Should().Be(orderStatus);
        }

        [Fact]
        public async Task GetOrderAsync_Should_ReturnEntityNotFoundException_When_NoOrder()
        {
            // Arrange
            var orderId = _fixture.Create<Guid>();

            // Act
            var response = await _client.GetAsync<OrderDto>($"/api/v1/orders/{orderId}");

            // Assert
            response.EnsureStatusCode((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetOrderAsync_Should_ReturnOrder()
        {
            // Arrange
            var orderStatus = _fixture.Create<OrderStatus>();
            var orderId = SeedHelper.CreateOrderWithItem(_dbContext, _fixture, orderStatus);

            // Act
            var response = await _client.GetAsync<OrderDto>($"/api/v1/orders/{orderId}");

            // Assert
            response.EnsureStatusCode((int)HttpStatusCode.OK);
            response.Body.Id.Should().Be(orderId);
            response.Body.Status.Should().Be(orderStatus);
        }
    }
}
