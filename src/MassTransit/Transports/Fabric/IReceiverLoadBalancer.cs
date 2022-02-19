namespace MassTransit.Transports.Fabric
{
    public interface IReceiverLoadBalancer<in T>
        where T : class
    {
        IMessageReceiver<T> SelectReceiver(T message);
    }
}
