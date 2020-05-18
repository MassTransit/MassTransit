namespace MassTransit.RabbitMqTransport
{
    public interface IQueueBindingConfigurator :
        IQueueConfigurator,
        IExchangeBindingConfigurator
    {
    }
}
