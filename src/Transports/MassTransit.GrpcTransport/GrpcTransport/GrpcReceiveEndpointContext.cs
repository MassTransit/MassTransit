namespace MassTransit.GrpcTransport
{
    using Fabric;
    using Transports;
    using Transports.Fabric;


    public interface GrpcReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IMessageFabric<NodeContext, GrpcTransportMessage> MessageFabric { get; }

        IGrpcTransportProvider TransportProvider { get; }

        void ConfigureTopology(NodeContext nodeContext);
    }
}
