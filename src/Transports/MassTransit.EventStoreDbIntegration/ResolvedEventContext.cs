using System;

namespace MassTransit.EventStoreDbIntegration
{
    public interface ResolvedEventContext
    {
        /// <summary>
        /// The stream that this event belongs to.
        /// </summary>
        string EventStreamId { get; }
        /// <summary>
        /// The type of this event.
        /// </summary>
        string EventType { get; }
        /// <summary>
        /// The position of this event within the transaction log.
        /// </summary>
        ulong? CommitPosition { get; }
        /// <summary>
        /// The position of this event within the stream to which it belongs.
        /// </summary>
        ulong EventNumber { get; }
        /// <summary>
        /// A UTC DateTime representing when this event was created in the system.
        /// </summary>
        DateTime TimeStamp { get; }
        /// <summary>
        /// The raw bytes representing the metadata of this event.
        /// </summary>
        byte[] Metadata { get; }
    }
}
