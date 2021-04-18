using System;

namespace MassTransit.EventStoreDbIntegration
{
    public interface EventRecordContext
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
        /// For an $all stream subscription, it is the position of this event within the transaction log. 
        /// Otherwise, it is the StreamPosition of this event in the stream to which it belongs.
        /// </summary>
        ulong Offset { get; }
        /// <summary>
        /// The StreamPosition of this event in the stream.
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
