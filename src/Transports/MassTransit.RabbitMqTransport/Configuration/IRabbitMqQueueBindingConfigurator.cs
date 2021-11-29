namespace MassTransit
{
    public interface IRabbitMqQueueBindingConfigurator :
        IRabbitMqQueueConfigurator,
        IRabbitMqExchangeBindingConfigurator
    {
    }
}
