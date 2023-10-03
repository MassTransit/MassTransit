#nullable enable
namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;


    public class InMemoryTransportMessage
    {
        static long _nextSequenceNumber;

        public InMemoryTransportMessage(Guid messageId, byte[] body, string contentType)
        {
            Headers = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            MessageId = messageId;
            Body = body;

            Headers[MessageHeaders.MessageId] = messageId.ToString();
            Headers[MessageHeaders.ContentType] = contentType;

            SequenceNumber = Interlocked.Increment(ref _nextSequenceNumber);
        }

        public long SequenceNumber { get; }

        public Guid MessageId { get; }

        public byte[] Body { get; }

        public int DeliveryCount { get; set; }

        public IDictionary<string, object> Headers { get; }

        public TimeSpan? Delay { get; set; }
        public string? RoutingKey { get; set; }
    }
}
