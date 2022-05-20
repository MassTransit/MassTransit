namespace MassTransit.GrpcTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Fabric;
    using Transports.Fabric;


    public class RemoteNodeMessageReceiver :
        IMessageReceiver<GrpcTransportMessage>
    {
        readonly IGrpcNode _node;
        readonly string _queueName;
        readonly long _receiverId;

        public RemoteNodeMessageReceiver(IGrpcNode node, string queueName, long receiverId)
        {
            _node = node;
            _queueName = queueName;
            _receiverId = receiverId;
        }

        public async Task Deliver(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            var transportMessage = new TransportMessage(message.Message)
            {
                Deliver =
                {
                    Receiver = new ReceiverDestination
                    {
                        QueueName = _queueName,
                        ReceiverId = _receiverId
                    }
                }
            };

            var grpcTransportMessage = new GrpcTransportMessage(transportMessage, message.Host);

            await _node.DeliverMessage(grpcTransportMessage).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("remote");
            scope.Add("nodeAddress", _node.NodeAddress);
        }
    }
}
