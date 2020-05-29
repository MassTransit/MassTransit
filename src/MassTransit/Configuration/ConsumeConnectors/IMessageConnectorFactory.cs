namespace MassTransit.ConsumeConnectors
{
    public interface IMessageConnectorFactory
    {
        IConsumerMessageConnector<T> CreateConsumerConnector<T>()
            where T : class;

        IInstanceMessageConnector<T> CreateInstanceConnector<T>()
            where T : class;
    }
}
