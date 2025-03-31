using Amazon.SQS;
using Amazon.SQS.Model;
using Com.Store.Orders.Domain.Services.Events;
using Com.Store.Orders.Domain.Services.Exceptions;
using Com.Store.Orders.Domain.Services.Options;
using Com.Store.Orders.Domain.Services.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Com.Store.Orders.Domain.Services.Services
{
    public class SqsEventProducingService : IEventProducingService
    {
        private readonly IAmazonSQS _client;
        private readonly ILogger<SqsEventProducingService> _logger;
        private readonly AwsMessagingOptions _messagingOptions;

        public SqsEventProducingService(
            IAmazonSQS client,
            ILoggerFactory loggerFactory,
            IOptions<AwsMessagingOptions> options)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<SqsEventProducingService>();
            _messagingOptions = options.Value;
        }

        public async Task<Guid> ProduceAsync<T>(T message, CancellationToken ct)
        {
            if (!_messagingOptions.Queues.TryGetValue(typeof(T).Name, out var queueUrl))
            {
                _logger.LogError($"Missing configuration for {typeof(T).Name} event.");
                throw new MissingQueueConfigurationException($"Missing configuration for {typeof(T).Name} event.");
            }

            var @event = new EventBase<T>()
            {
                CorrelationId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Payload = message,
                Source = _messagingOptions.Source,
                Timestamp = DateTime.UtcNow,
                Type = typeof(T).Name
            };

            var request = new SendMessageRequest()
            {
                QueueUrl = queueUrl,
                MessageBody = JsonSerializer.Serialize(@event)
            };

            await _client.SendMessageAsync(request, ct);
            return @event.CorrelationId;
        }
    }
}
