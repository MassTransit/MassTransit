namespace MassTransit
{
    /// <summary>
    /// Connect an observer that is notified when a message is sent to an endpoint
    /// </summary>
    public interface ISendObserverConnector
    {
        ConnectHandle ConnectSendObserver(ISendObserver observer);
    }
}
