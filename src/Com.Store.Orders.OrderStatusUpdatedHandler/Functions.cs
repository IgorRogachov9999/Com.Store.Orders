using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Com.Store.Orders.Domain.Services.Services.Contracts;
using Amazon.Lambda.SQSEvents;
using System.Text.Json;
using Com.Store.Orders.Domain.Services.Events;
using Com.Store.Orders.Domain.Services.Dto;
using static Amazon.Lambda.SQSEvents.SQSBatchResponse;
using Amazon.SQS;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

// Function namesapce max length is 127 chars.
namespace Com.Store.Orders.OSUH;

public class Functions
{
    private readonly IOrderStatusUpdatedHandlerService _orderStatusUpdatedHandlerService;

    public Functions(IOrderStatusUpdatedHandlerService orderStatusUpdatedHandlerService)
    {
        _orderStatusUpdatedHandlerService = orderStatusUpdatedHandlerService;
    }

    [LambdaFunction()]
    public async Task<SQSBatchResponse> HandleAsync(SQSEvent input, ILambdaContext context)
    {
        var failedItems = new List<BatchItemFailure>();

        foreach (var record in input.Records)
        {
            try
            {
                await ProcessMessageAsync(record, context);
            }
            catch (Exception)
            {
                failedItems.Add(new BatchItemFailure()
                {
                    ItemIdentifier = record.MessageId
                });
            }
        }

        return new SQSBatchResponse(failedItems);
    }

    private Task ProcessMessageAsync(SQSEvent.SQSMessage record, ILambdaContext context)
    {
        var message = JsonSerializer.Deserialize<EventBase<OrderStatusUpdated>>(record.Body);

        if (message == null)
        {
            throw new AmazonSQSException($"Invalid message with id = {record.MessageId}");
        }

        var @event = new OrderStatusUpdatedDto()
        {
            Timestamp = message.Timestamp,
            OrderId = message.Payload.OrderId,
            Status = message.Payload.Status
        };
        return _orderStatusUpdatedHandlerService.HandleAsync(@event, CancellationToken.None);
    }
}