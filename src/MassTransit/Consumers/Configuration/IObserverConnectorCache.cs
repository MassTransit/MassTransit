namespace MassTransit.Configuration
{
    public interface IObserverConnectorCache<T>
        where T : class
    {
        IObserverConnector<T> Connector { get; }
    }
}
