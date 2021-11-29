namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public interface EventHubConsumeContext
    {
        DateTimeOffset EnqueuedTime { get; }
        long Offset { get; }
        string PartitionId { get; }
        string PartitionKey { get; }
        long SequenceNumber { get; }
        IReadOnlyDictionary<string, object> SystemProperties { get; }
        IDictionary<string, object> Properties { get; }
    }
}
