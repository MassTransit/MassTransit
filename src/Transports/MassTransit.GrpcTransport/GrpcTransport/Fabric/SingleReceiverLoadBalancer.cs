namespace MassTransit.GrpcTransport.Fabric
{
    public class SingleReceiverLoadBalancer :
        IReceiverLoadBalancer
    {
        readonly IMessageReceiver _receiver;

        public SingleReceiverLoadBalancer(IMessageReceiver receiver)
        {
            _receiver = receiver;
        }

        public IMessageReceiver SelectReceiver(GrpcTransportMessage message)
        {
            return _receiver;
        }
    }
}
