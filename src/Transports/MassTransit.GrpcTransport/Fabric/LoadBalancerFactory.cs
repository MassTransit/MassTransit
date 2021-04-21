namespace MassTransit.GrpcTransport.Fabric
{
    public delegate IConsumerLoadBalancer LoadBalancerFactory(IGrpcQueueConsumer[] consumers);
}
