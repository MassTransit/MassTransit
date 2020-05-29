namespace MassTransit.Saga
{
    using Connectors;


    public interface ISagaConnectorFactory
    {
        ISagaMessageConnector<T> CreateMessageConnector<T>()
            where T : class, ISaga;
    }
}
