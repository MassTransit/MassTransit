namespace MassTransit.GrpcTransport.Fabric
{
    using Contexts;
    using GreenPipes;


    public interface IMessageFabricObserver
    {
        void ExchangeDeclared(NodeContext context, string name);
        void ExchangeBindingCreated(NodeContext context, string source, string destination, string routingKey = default);
        void QueueDeclared(NodeContext context, string name);
        void QueueBindingCreated(NodeContext context, string source, string destination, string routingKey = default);

        ConnectHandle ConsumerConnected(NodeContext context, ConnectHandle handle, string queueName);
    }
}
