namespace MassTransit.GrpcTransport.Fabric
{
    public interface IReceiverLoadBalancer
    {
        IMessageReceiver SelectReceiver(GrpcTransportMessage message);
    }
}
