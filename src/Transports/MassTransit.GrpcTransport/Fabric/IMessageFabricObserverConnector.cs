namespace MassTransit.GrpcTransport.Fabric
{
    using GreenPipes;


    public interface IMessageFabricObserverConnector
    {
        ConnectHandle ConnectMessageFabricObserver(IMessageFabricObserver observer);
    }
}