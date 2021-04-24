namespace MassTransit.GrpcTransport.Fabric
{
    public delegate IReceiverLoadBalancer LoadBalancerFactory(IMessageReceiver[] consumers);
}
