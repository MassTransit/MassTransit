#nullable enable
namespace MassTransit.MongoDbIntegration.Outbox
{
    using System;
    using MongoDB.Bson;


    /// <summary>
    /// Used by the sweeper to track the state of an outbox, to ensure that it is properly locked
    /// across sweeper instances to ensure in-order delivery of messages from the outbox.
    /// </summary>
    public class OutboxState
    {
        /// <summary>
        /// Assigned when the scope is created for an outbox
        /// </summary>
        public Guid OutboxId { get; set; }

        public ObjectId LockToken { get; set; }

        /// <summary>
        /// Available for optimistic concurrency
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// When all messages in the outbox were delivered to the transport
        /// </summary>
        public DateTime? Delivered { get; set; }

        /// <summary>
        /// The last sequence number that was successfully delivered to the transport
        /// </summary>
        public long? LastSequenceNumber { get; set; }
    }
}
