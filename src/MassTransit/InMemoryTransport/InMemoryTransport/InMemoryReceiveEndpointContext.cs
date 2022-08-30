namespace MassTransit.InMemoryTransport
{
    using Transports;
    using Transports.Fabric;


    public interface InMemoryReceiveEndpointContext :
        ReceiveEndpointContext
    {
        ISendTopology Send { get; }

        IMessageFabric<InMemoryTransportContext, InMemoryTransportMessage> MessageFabric { get; }

        InMemoryTransportContext TransportContext { get; }

        void ConfigureTopology();
    }
}
