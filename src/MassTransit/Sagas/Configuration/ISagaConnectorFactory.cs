namespace MassTransit.Configuration
{
    public interface ISagaConnectorFactory
    {
        ISagaMessageConnector<T> CreateMessageConnector<T>()
            where T : class, ISaga;
    }
}
