namespace MassTransit.ConsumeConnectors
{
    public interface IConsumerConnectorCache
    {
        IConsumerConnector Connector { get; }
    }
}
