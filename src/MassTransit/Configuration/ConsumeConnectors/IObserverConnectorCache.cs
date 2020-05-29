namespace MassTransit.ConsumeConnectors
{
    public interface IObserverConnectorCache<T>
        where T : class
    {
        IObserverConnector<T> Connector { get; }
    }
}
