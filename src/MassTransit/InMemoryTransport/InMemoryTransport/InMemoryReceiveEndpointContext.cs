namespace MassTransit.InMemoryTransport
{
    using Fabric;
    using Transports;


    public interface InMemoryReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IMessageFabric MessageFabric { get; }

        void ConfigureTopology();
    }
}
