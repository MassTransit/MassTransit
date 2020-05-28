namespace MassTransit.Riders
{
    using GreenPipes;


    public interface IRiderObserverConnector
    {
        ConnectHandle ConnectRiderObserver(IRiderObserver observer);
    }
}
