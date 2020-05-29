namespace MassTransit.ConsumeConnectors
{
    public interface IInstanceConnectorCache<T>
        where T : class
    {
        IInstanceConnector Connector { get; }
    }
}
