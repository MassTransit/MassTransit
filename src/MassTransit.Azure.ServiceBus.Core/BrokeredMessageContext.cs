namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// The context of a Message from AzureServiceBus - gives access to the transport
    /// message when requested.
    /// </summary>
    public interface BrokeredMessageContext
    {
        int DeliveryCount { get; }
        string Label { get; }
        long SequenceNumber { get; }
        long EnqueuedSequenceNumber { get; }
        string LockToken { get; }
        DateTime LockedUntil { get; }
        string SessionId { get; }
        long Size { get; }
        string To { get; }
        string ReplyToSessionId { get; }
        string PartitionKey { get; }
        string ViaPartitionKey { get; }
        string ReplyTo { get; }
        DateTime EnqueuedTime { get; }
        DateTime ScheduledEnqueueTime { get; }
        IDictionary<string, object> Properties { get; }
        TimeSpan TimeToLive { get; }
        string CorrelationId { get; }
        string MessageId { get; }
        DateTime ExpiresAt { get; }
    }
}
