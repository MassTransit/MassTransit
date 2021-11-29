namespace MassTransit.Configuration
{
    public interface IConsumerConnectorCache
    {
        IConsumerConnector Connector { get; }
    }
}
