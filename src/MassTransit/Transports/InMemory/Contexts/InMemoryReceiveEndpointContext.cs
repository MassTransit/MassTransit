namespace MassTransit.Transports.InMemory.Contexts
{
    using Context;
    using Fabric;


    public interface InMemoryReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IMessageFabric MessageFabric { get; }

        void ConfigureTopology();
    }
}
