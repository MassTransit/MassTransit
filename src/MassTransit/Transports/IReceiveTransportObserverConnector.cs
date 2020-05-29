namespace MassTransit.Transports
{
    using GreenPipes;


    public interface IReceiveTransportObserverConnector
    {
        ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer);
    }
}
