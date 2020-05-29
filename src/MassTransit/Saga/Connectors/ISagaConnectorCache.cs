namespace MassTransit.Saga.Connectors
{
    public interface ISagaConnectorCache
    {
        ISagaConnector Connector { get; }
    }
}
