namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;


    public interface IGrpcQueue :
        IMessageSink<GrpcTransportMessage>,
        IAsyncDisposable
    {
        ConnectHandle ConnectConsumer(NodeContext nodeContext, IGrpcQueueConsumer consumer);

        Task Send(GrpcTransportMessage message, CancellationToken cancellationToken = default);
    }
}
