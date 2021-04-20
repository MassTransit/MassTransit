namespace MassTransit.GrpcTransport.Integration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Fabric;


    public class GrpcRemoteConsumer :
        IGrpcQueueConsumer
    {
        readonly IGrpcNode _node;

        public GrpcRemoteConsumer(IGrpcNode node)
        {
            _node = node;
        }

        public async Task Consume(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            await _node.DeliverMessage(message).ConfigureAwait(false);
        }
    }
}
