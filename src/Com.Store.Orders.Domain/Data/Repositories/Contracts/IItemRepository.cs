namespace Com.Store.Orders.Domain.Data.Repositories.Contracts
{
    public interface IItemRepository
    {
        Task<bool> ContainsAsync(Guid[] ids, CancellationToken ct);
    }
}
