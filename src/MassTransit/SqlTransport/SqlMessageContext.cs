namespace MassTransit
{
    using System;
    using SqlTransport;


    public interface SqlMessageContext :
        RoutingKeyConsumeContext,
        PartitionKeyConsumeContext
    {
        SqlTransportMessage TransportMessage { get; }

        Guid TransportMessageId { get; }
        long DeliveryMessageId { get; }

        string QueueName { get; }

        Guid? ConsumerId { get; }
        Guid? LockId { get; }

        short Priority { get; }
        DateTime EnqueueTime { get; }
        int DeliveryCount { get; }
    }
}
