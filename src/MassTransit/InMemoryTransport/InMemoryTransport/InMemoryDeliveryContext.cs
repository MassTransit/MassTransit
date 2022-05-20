#nullable enable
namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Transports.Fabric;


    public class InMemoryDeliveryContext :
        DeliveryContext<InMemoryTransportMessage>
    {
        readonly HashSet<IMessageSink<InMemoryTransportMessage>> _delivered;

        public InMemoryDeliveryContext(InMemoryTransportMessage message, CancellationToken cancellationToken)
        {
            Message = message;
            CancellationToken = cancellationToken;

            _delivered = new HashSet<IMessageSink<InMemoryTransportMessage>>();
        }

        public CancellationToken CancellationToken { get; }

        public InMemoryTransportMessage Message { get; }
        public string? RoutingKey => Message.RoutingKey;
        public DateTime? EnqueueTime => Message.Delay.HasValue ? DateTime.UtcNow + Message.Delay : default;
        public long? ReceiverId => default;

        public bool WasAlreadyDelivered(IMessageSink<InMemoryTransportMessage> sink)
        {
            return _delivered.Contains(sink);
        }

        public void Delivered(IMessageSink<InMemoryTransportMessage> sink)
        {
            _delivered.Add(sink);
        }
    }
}
