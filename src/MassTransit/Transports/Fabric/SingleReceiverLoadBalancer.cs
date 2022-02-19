namespace MassTransit.Transports.Fabric
{
    public class SingleReceiverLoadBalancer<T> :
        IReceiverLoadBalancer<T>
        where T : class
    {
        readonly IMessageReceiver<T> _receiver;

        public SingleReceiverLoadBalancer(IMessageReceiver<T> receiver)
        {
            _receiver = receiver;
        }

        public IMessageReceiver<T> SelectReceiver(T message)
        {
            return _receiver;
        }
    }
}
