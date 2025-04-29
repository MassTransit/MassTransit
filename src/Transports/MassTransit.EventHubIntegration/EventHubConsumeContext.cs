namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public interface EventHubConsumeContext :
        PartitionKeyConsumeContext
    {
        DateTimeOffset EnqueuedTime { get; }

        [Obsolete("The Event Hubs service does not guarantee a numeric offset for all resource configurations.  Please use 'OffsetString' instead.")]
        long Offset { get; }

        string OffsetString { get; }
        string PartitionId { get; }
        long SequenceNumber { get; }
        IReadOnlyDictionary<string, object> SystemProperties { get; }
        IDictionary<string, object> Properties { get; }
    }
}
