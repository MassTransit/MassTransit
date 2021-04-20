namespace MassTransit.GrpcTransport.Fabric
{
    public interface IConsumerLoadBalancer
    {
        IGrpcQueueConsumer SelectConsumer(GrpcTransportMessage message);
    }
}
