namespace MassTransit
{
    public interface IReceiveEndpointObserverConnector
    {
        ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer);
    }
}
