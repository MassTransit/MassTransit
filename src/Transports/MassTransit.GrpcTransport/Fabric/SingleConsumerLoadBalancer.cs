namespace MassTransit.GrpcTransport.Fabric
{
    public class SingleConsumerLoadBalancer :
        IConsumerLoadBalancer
    {
        readonly IGrpcQueueConsumer _consumer;

        public SingleConsumerLoadBalancer(IGrpcQueueConsumer consumer)
        {
            _consumer = consumer;
        }

        public IGrpcQueueConsumer SelectConsumer(GrpcTransportMessage message)
        {
            return _consumer;
        }
    }
}
