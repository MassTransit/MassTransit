namespace MassTransit.Transports.InMemory.Fabric
{
    using System.Collections.Generic;


    public class InMemoryDeliveryContext :
        DeliveryContext<InMemoryTransportMessage>
    {
        readonly HashSet<IMessageSink<InMemoryTransportMessage>> _delivered;
        
        public InMemoryDeliveryContext(InMemoryTransportMessage message)
        {
            Package = message;
            
            _delivered = new HashSet<IMessageSink<InMemoryTransportMessage>>();
        }

        public bool WasAlreadyDelivered(IMessageSink<InMemoryTransportMessage> sink)
        {
            return _delivered.Contains(sink);
        }

        public void Delivered(IMessageSink<InMemoryTransportMessage> sink)
        {
            _delivered.Add(sink);
        }

        public InMemoryTransportMessage Package { get; }
    }
}