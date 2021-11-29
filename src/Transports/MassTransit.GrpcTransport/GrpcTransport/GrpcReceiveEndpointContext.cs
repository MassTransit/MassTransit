namespace MassTransit.GrpcTransport
{
    using Fabric;
    using Transports;


    public interface GrpcReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IMessageFabric MessageFabric { get; }

        IGrpcTransportProvider TransportProvider { get; }

        void ConfigureTopology(NodeContext nodeContext);
    }
}
