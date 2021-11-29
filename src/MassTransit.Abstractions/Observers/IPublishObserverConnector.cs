namespace MassTransit
{
    /// <summary>
    /// Connect an observer that is notified when a message is sent to an endpoint
    /// </summary>
    public interface IPublishObserverConnector
    {
        ConnectHandle ConnectPublishObserver(IPublishObserver observer);
    }
}
