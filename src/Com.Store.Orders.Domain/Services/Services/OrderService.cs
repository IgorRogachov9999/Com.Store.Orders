using AutoMapper;
using Com.Store.Orders.Domain.Data.Entities;
using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Data.Models.Pagination;
using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Com.Store.Orders.Domain.Services.Dto;
using Com.Store.Orders.Domain.Services.Events;
using Com.Store.Orders.Domain.Services.Exceptions;
using Com.Store.Orders.Domain.Services.Services.Contracts;

namespace Com.Store.Orders.Domain.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IEventProducingService _eventProducer;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IItemRepository itemRepository,
            IEventProducingService eventProducer,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _eventProducer = eventProducer;
            _mapper = mapper;
        }

        public async Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdAsync(id, ct);

            if (order == null)
            {
                throw new DomainEntityNotFoundException($"Order with id = {id} does not exist.");
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<PageModel<OrderDto>> GetOrdersAsync(PaginationSettingsModel paginationSettings, CancellationToken ct)
        {
            var orders = await _orderRepository.GetOrdersAsync(paginationSettings, ct);
            return _mapper.Map<PageModel<OrderDto>>(orders);
        }

        public async Task<OrderStatus> GetStatusByOrderIdAsync(Guid id, CancellationToken ct)
        {
            var status = await _orderRepository.GetStatusByOrderIdAsync(id, ct);

            if (!status.HasValue)
            {
                throw new DomainEntityNotFoundException($"Order with id = {id} does not exist.");
            }

            return status.Value;
        }

        public async Task<Guid> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken ct)
        {
            await ValidateOrderItemsAsync(createOrderDto.Items, ct);

            var order = _mapper.Map<Order>(createOrderDto);
            order.CreatedAt = DateTime.UtcNow;

            await _orderRepository.CreateOrderAsync(order, ct);
            return order.Id;
        }

        public async Task<Guid> UpdateOrderStatusAsync(Guid orderId, OrderStatus orderStatus, CancellationToken ct)
        {
            var message = new OrderStatusUpdated()
            {
                OrderId = orderId,
                Status = orderStatus
            };
            var correlationId = await _eventProducer.ProduceAsync(message, ct);
            return correlationId;
        }

        private async Task ValidateOrderItemsAsync(IEnumerable<CreateOrderItemDto> createOrderItemDto, CancellationToken ct)
        {
            var itemIds = createOrderItemDto.Select(x => x.Id).ToArray();
            var containsAllIds = await _itemRepository.ContainsAsync(itemIds, ct);

            if (!containsAllIds)
            {
                throw new DomainEntityNotFoundException("Some of items are not exist.");
            }
        }
    }
}
