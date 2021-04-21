namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;


    public interface IGrpcQueue :
        IMessageSink<GrpcTransportMessage>,
        IAsyncDisposable
    {
        TopologyHandle ConnectConsumer(NodeContext nodeContext, IGrpcQueueConsumer consumer);

        Task Send(GrpcTransportMessage message, CancellationToken cancellationToken = default);
    }
}
