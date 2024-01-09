namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public interface EventHubConsumeContext :
        PartitionKeyConsumeContext
    {
        DateTimeOffset EnqueuedTime { get; }
        long Offset { get; }
        string PartitionId { get; }
        long SequenceNumber { get; }
        IReadOnlyDictionary<string, object> SystemProperties { get; }
        IDictionary<string, object> Properties { get; }
    }
}
