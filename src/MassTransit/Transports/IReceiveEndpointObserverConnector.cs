namespace MassTransit.Transports
{
    using GreenPipes;


    public interface IReceiveEndpointObserverConnector
    {
        ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer);
    }
}
