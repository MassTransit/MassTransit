namespace MassTransit.GrpcTransport.Contexts
{
    using Context;
    using Fabric;
    using Integration;


    public interface GrpcReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IMessageFabric MessageFabric { get; }

        IGrpcTransportProvider TransportProvider { get; }

        void ConfigureTopology(NodeContext nodeContext);
    }
}
