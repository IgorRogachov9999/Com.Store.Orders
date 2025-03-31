namespace Com.Store.Orders.Domain.Services.Services.Contracts
{
    public interface IEventProducingService
    {
        public Task<Guid> ProduceAsync<T>(T message, CancellationToken ct);
    }
}
