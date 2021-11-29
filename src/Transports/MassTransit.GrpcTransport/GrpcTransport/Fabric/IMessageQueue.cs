namespace MassTransit.GrpcTransport.Fabric
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IMessageQueue :
        IMessageSink<GrpcTransportMessage>
    {
        TopologyHandle ConnectMessageReceiver(NodeContext nodeContext, IMessageReceiver receiver);

        Task Send(GrpcTransportMessage message, CancellationToken cancellationToken = default);
    }
}
