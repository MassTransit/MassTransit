namespace MassTransit
{
    public interface IReceiveTransportObserverConnector
    {
        ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer);
    }
}
