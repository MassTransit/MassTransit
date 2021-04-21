namespace MassTransit.GrpcTransport.Integration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Fabric;


    public class GrpcRemoteConsumer :
        IGrpcQueueConsumer
    {
        readonly IGrpcNode _node;

        public GrpcRemoteConsumer(IGrpcNode node, string queueName, long consumerId)
        {
            _node = node;

            QueueName = queueName;
            ConsumerId = consumerId;
        }

        long ConsumerId { get; }
        string QueueName { get; }

        public async Task Consume(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            var transportMessage = new TransportMessage(message.Message)
            {
                Deliver =
                {
                    Consumer = new ConsumerDestination
                    {
                        QueueName = QueueName,
                        ConsumerId = ConsumerId
                    }
                }
            };

            var grpcTransportMessage = new GrpcTransportMessage(transportMessage, message.Host);

            await _node.DeliverMessage(grpcTransportMessage).ConfigureAwait(false);
        }
    }
}
