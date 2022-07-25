#nullable enable
namespace MassTransit.MongoDbIntegration.Outbox
{
    using System;
    using MongoDB.Bson;


    public class InboxState
    {
        /// <summary>
        /// Unique ID for the inbox state
        /// </summary>
        public Guid Id { get; set; }

        public ObjectId LockToken { get; set; }

        /// <summary>
        /// Version for updating
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The MessageId of the incoming message
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// And MD5 hash of the endpoint name + consumer type
        /// </summary>
        public Guid ConsumerId { get; set; }

        /// <summary>
        /// When the message was first received
        /// </summary>
        public DateTime Received { get; set; }

        /// <summary>
        /// How many times the message has been received
        /// </summary>
        public int ReceiveCount { get; set; }

        /// <summary>
        /// If present, when the message expires (from the message header)
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// When the message was consumed, successfully
        /// </summary>
        public DateTime? Consumed { get; set; }

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
