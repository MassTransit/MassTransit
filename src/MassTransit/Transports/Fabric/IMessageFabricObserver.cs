namespace MassTransit.Transports.Fabric
{
    public interface IMessageFabricObserver<in TContext>
        where TContext : class
    {
        void ExchangeDeclared(TContext context, string name, ExchangeType exchangeType);

        void ExchangeBindingCreated(TContext context, string source, string destination, string routingKey = default);

        void QueueDeclared(TContext context, string name);

        void QueueBindingCreated(TContext context, string source, string destination);

        TopologyHandle ConsumerConnected(TContext context, TopologyHandle handle, string queueName);
    }
}
