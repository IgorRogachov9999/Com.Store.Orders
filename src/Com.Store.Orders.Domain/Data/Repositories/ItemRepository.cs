using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Com.Store.Orders.Domain.Data.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly OrdersDbContext _dbContext;

        public ItemRepository(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ContainsAsync(Guid[] ids, CancellationToken ct)
        {
            return await _dbContext.Items
                .Where(x => !x.IsDeleted && ids.Contains(x.Id))
                .Select(x => x.Id)
                .CountAsync(ct) == ids.Length;
        }
    }
}
