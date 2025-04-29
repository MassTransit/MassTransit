namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public interface EventDataContext
    {
        DateTimeOffset EnqueuedTime { get; }

        [Obsolete("The Event Hubs service does not guarantee a numeric offset for all resource configurations.  Please use 'OffsetString' instead.")]
        long Offset { get; }

        /// <summary>
        /// Replaces Offset in current versions of the Azure SDK
        /// </summary>
        string OffsetString { get; }

        string PartitionKey { get; }
        long SequenceNumber { get; }
        IReadOnlyDictionary<string, object> SystemProperties { get; }
        IDictionary<string, object> Properties { get; }
    }
}
