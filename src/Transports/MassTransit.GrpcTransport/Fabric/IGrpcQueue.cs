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
        TopologyHandle ConnectMessageReceiver(NodeContext nodeContext, IMessageReceiver receiver);

        Task Send(GrpcTransportMessage message, CancellationToken cancellationToken = default);
    }
}
