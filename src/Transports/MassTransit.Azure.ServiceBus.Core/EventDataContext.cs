namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Collections.Generic;


    public interface EventDataContext
    {
        DateTime EnqueuedTime { get; }
        string Offset { get; }
        string PartitionKey { get; }
        long SequenceNumber { get; }
        IDictionary<string, object> SystemProperties { get; }
        IDictionary<string, object> Properties { get; }
    }
}
