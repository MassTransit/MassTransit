namespace MassTransit.GrpcTransport.Integration
{
    using System;
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
            if (message.SourceAddress.GetLeftPart(UriPartial.Authority) == _node.NodeAddress.GetLeftPart(UriPartial.Authority))
                return;

            await _node.DeliverMessage(message).ConfigureAwait(false);
        }
    }
}
