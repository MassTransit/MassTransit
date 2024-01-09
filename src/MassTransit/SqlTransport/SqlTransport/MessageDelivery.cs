#nullable enable
namespace MassTransit.SqlTransport
{
    using System;


    public class MessageDelivery
    {
        public int QueueId { get; set; }
        public int Priority { get; set; }
        public DateTimeOffset EnqueueTime { get; set; }
        public Guid? ConsumerId { get; set; }
        public Guid TransportMessageId { get; set; }
        public DateTimeOffset? ExpirationTime { get; set; }
        public int DeliveryCount { get; set; }
        public int MaxDeliveryCount { get; set; }
        public DateTimeOffset? LastDelivered { get; set; }
        public long SessionNumber { get; set; }
        public string? ReplyToSessionId { get; set; }
        public string? GroupId { get; set; }
        public int? GroupSequenceNumber { get; set; }
        public string? TransportHeaders { get; set; }
    }
}
