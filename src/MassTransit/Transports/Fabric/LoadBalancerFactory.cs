namespace MassTransit.Transports.Fabric
{
    public delegate IReceiverLoadBalancer<T> LoadBalancerFactory<T>(IMessageReceiver<T>[] consumers)
        where T : class;
}
