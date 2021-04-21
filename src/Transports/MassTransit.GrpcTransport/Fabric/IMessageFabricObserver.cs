namespace MassTransit.GrpcTransport.Fabric
{
    using Contexts;
    using Contracts;


    public interface IMessageFabricObserver
    {
        void ExchangeDeclared(NodeContext context, string name, ExchangeType exchangeType);

        void ExchangeBindingCreated(NodeContext context, string source, string destination, string routingKey = default);

        void QueueDeclared(NodeContext context, string name);

        void QueueBindingCreated(NodeContext context, string source, string destination);

        TopologyHandle ConsumerConnected(NodeContext context, TopologyHandle handle, string queueName);
    }
}
