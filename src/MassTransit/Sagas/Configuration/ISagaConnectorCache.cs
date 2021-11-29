namespace MassTransit.Configuration
{
    public interface ISagaConnectorCache
    {
        ISagaConnector Connector { get; }
    }
}
