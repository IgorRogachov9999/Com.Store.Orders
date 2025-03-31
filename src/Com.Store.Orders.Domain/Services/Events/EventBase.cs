namespace Com.Store.Orders.Domain.Services.Events
{
    public class EventBase<T>
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

        public string Source { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid CorrelationId { get; set; }

        public T Payload { get; set; }
    }
}
