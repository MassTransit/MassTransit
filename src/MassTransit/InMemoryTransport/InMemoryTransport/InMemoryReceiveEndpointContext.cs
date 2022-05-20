namespace MassTransit.InMemoryTransport
{
    using Transports;
    using Transports.Fabric;


    public interface InMemoryReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IMessageFabric<InMemoryTransportContext, InMemoryTransportMessage> MessageFabric { get; }

        InMemoryTransportContext TransportContext { get; }

        void ConfigureTopology();
    }
}
