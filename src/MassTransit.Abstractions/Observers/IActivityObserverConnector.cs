namespace MassTransit
{
    public interface IActivityObserverConnector
    {
        ConnectHandle ConnectActivityObserver(IActivityObserver observer);
    }
}
