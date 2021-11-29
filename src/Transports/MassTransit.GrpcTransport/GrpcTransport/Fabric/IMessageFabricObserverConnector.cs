namespace MassTransit.GrpcTransport.Fabric
{
    public interface IMessageFabricObserverConnector
    {
        ConnectHandle ConnectMessageFabricObserver(IMessageFabricObserver observer);
    }
}
