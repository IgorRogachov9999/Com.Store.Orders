using Com.Store.Orders.Domain.Data.Entities;
using Com.Store.Orders.Domain.Data.Enums;
using Com.Store.Orders.Domain.Data.Extensions;
using Com.Store.Orders.Domain.Data.Models.Pagination;
using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Com.Store.Orders.Domain.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _dbContext;

        public OrderRepository(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return _dbContext.Orders
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.Items)
                .ThenInclude(x => x.Item)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PageModel<Order>> GetOrdersAsync(PaginationSettingsModel paginationSettings, CancellationToken ct)
        {
            var query = _dbContext.Orders.Where(x => !x.IsDeleted);

            if (paginationSettings?.SortingOrder.HasValue == true)
            {
                query = query.Order(paginationSettings.SortingOrder.Value);
            }

            if (paginationSettings?.PageSize > 0 && paginationSettings?.PageNumber > 0)
            {
                query = query.Paginate(paginationSettings.PageSize, paginationSettings.PageNumber);
            }

            var orders = await query
                .Include(x => x.Items)
                .ThenInclude(x => x.Item)
                .AsNoTracking()
                .ToListAsync(ct);
            var count = await _dbContext.Orders.CountAsync(ct);

            return new PageModel<Order>()
            {
                Items = orders,
                PageNumber = paginationSettings?.PageNumber ?? 1,
                PageSize = paginationSettings?.PageSize ?? count,
                TotalCount = count,
                TotalPages = count / (paginationSettings?.PageSize ?? count)
            };
        }

        public Task<OrderStatus?> GetStatusByOrderIdAsync(Guid orderId, CancellationToken ct)
        {
            return _dbContext.Orders
                .Where(x => x.Id == orderId && !x.IsDeleted)
                .Select(x => (OrderStatus?)x.Status)
                .FirstOrDefaultAsync(ct);
        }

        public async Task CreateOrderAsync(Order order, CancellationToken ct)
        {
            await _dbContext.Orders.AddAsync(order, ct);
            await _dbContext.SaveChangesAsync(ct);
            _dbContext.Entry(order).State = EntityState.Detached;
        }

        public async Task<bool> ConfirmOrderAsync(Guid orderId, DateTime updateDate, CancellationToken ct)
        {
            var orderTableName = GetTableName<Order>();
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            var order = await _dbContext.Orders
                .FromSqlRaw($"SELECT * FROM {orderTableName} WHERE id = {{0}} AND is_deleted IS FALSE AND status = '{OrderStatus.Pending.ToString()}' FOR UPDATE", orderId)
                .Include(x => x.Items)
                .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(ct);

            if (order == null)
            {
                await transaction.RollbackAsync(ct);
                return false;
            }

            foreach (var orderItem in order.Items)
            {
                if (orderItem.Item.AvailableCount < orderItem.Quantity)
                {
                    await transaction.RollbackAsync(ct);
                    return false;
                }

                orderItem.Item.AvailableCount -= orderItem.Quantity;
            }

            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = updateDate;
            await transaction.CommitAsync(ct);
            return true;
        }

        public async Task<bool> ShipOrderAsync(Guid orderId, DateTime updateDate, CancellationToken ct)
        {
            var orderTableName = GetTableName<Order>();
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            var order = await _dbContext.Orders
                .FromSqlRaw($"SELECT * FROM {orderTableName} WHERE id = {{0}} AND is_deleted IS FALSE AND status = '{OrderStatus.Confirmed.ToString()}' FOR UPDATE", orderId)
                .FirstOrDefaultAsync(ct);

            if (order == null)
            {
                await transaction.RollbackAsync(ct);
                return false;
            }

            order.Status = OrderStatus.Shipped;
            order.UpdatedAt = updateDate;
            await transaction.CommitAsync(ct);
            return true;
        }

        private string GetTableName<T>()
        {
            var tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
            var schema = tableAttribute?.Schema ?? throw new InvalidOperationException("Schema does not exist.");
            var tableName = tableAttribute?.Name ?? throw new InvalidOperationException("Table does not exist.");
            return $"{schema}.{tableName}";
        }
    }
}
