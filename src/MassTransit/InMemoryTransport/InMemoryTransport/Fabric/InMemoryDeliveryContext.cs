namespace MassTransit.InMemoryTransport.Fabric
{
    using System.Collections.Generic;
    using System.Threading;


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
