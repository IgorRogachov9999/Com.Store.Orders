using Com.Store.Orders.Api.Constants;
using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Data.Models.Pagination;
using Com.Store.Orders.Domain.Services.Dto;
using Com.Store.Orders.Domain.Services.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Com.Store.Orders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}/status")]
        public async Task<ActionResult<OrderStatus>> GetOrderStatusAsync([FromRoute] Guid id, CancellationToken ct = default)
        {
            var orderStatus = await _orderService.GetStatusByOrderIdAsync(id, ct);
            return Ok(orderStatus);
        }

        [HttpGet("{id}")]
        [OutputCache(VaryByRouteValueNames = new[] { "id" })]
        public async Task<ActionResult<OrderDto>> GetOrderAsync([FromRoute] Guid id, CancellationToken ct = default)
        {
            var order = await _orderService.GetByIdAsync(id, ct);
            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<PageModel<OrderDto>>> GetOrdersAsync([FromQuery] PaginationSettingsModel paginationSettings, CancellationToken ct = default)
        {
            var orders = await _orderService.GetOrdersAsync(paginationSettings, ct);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateOrderAsync([FromBody] CreateOrderDto createOrderDto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderId = await _orderService.CreateOrderAsync(createOrderDto, ct);
            return Ok(orderId);
        }

        [Authorize(Policy = AuthorizationPolicyNames.RequireAdminOrConfirmingManagerRole)]
        [HttpPut("{id}/confirm")]
        public Task<ActionResult<Guid>> ConfirmOrderAsync([FromRoute] Guid id, CancellationToken ct = default)
        {
            return UpdateOrderStatusAsync(id, OrderStatus.Confirmed, ct);
        }

        [Authorize(Policy = AuthorizationPolicyNames.RequireAdminOrShippingManagerRole)]
        [HttpPut("{id}/ship")]
        public Task<ActionResult<Guid>> ShipOrderAsync([FromRoute] Guid id, CancellationToken ct = default)
        {
            return UpdateOrderStatusAsync(id, OrderStatus.Shipped, ct);
        }

        private async Task<ActionResult<Guid>> UpdateOrderStatusAsync(Guid id, OrderStatus status, CancellationToken ct)
        {
            var correlationId = await _orderService.UpdateOrderStatusAsync(id, status, ct);
            return Ok(correlationId);
        }
    }
}
