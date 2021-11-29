namespace MassTransit.GrpcTransport.Fabric
{
    using Contracts;
    using Util;


    public class MessageFabricObservable :
        Connectable<IMessageFabricObserver>,
        IMessageFabricObserver
    {
        public void ExchangeDeclared(NodeContext context, string name, ExchangeType exchangeType)
        {
            ForEach(x => x.ExchangeDeclared(context, name, exchangeType));
        }

        public void ExchangeBindingCreated(NodeContext context, string source, string destination, string routingKey)
        {
            ForEach(x => x.ExchangeBindingCreated(context, source, destination, routingKey));
        }

        public void QueueDeclared(NodeContext context, string name)
        {
            ForEach(x => x.QueueDeclared(context, name));
        }

        public void QueueBindingCreated(NodeContext context, string source, string destination)
        {
            ForEach(x => x.QueueBindingCreated(context, source, destination));
        }

        public TopologyHandle ConsumerConnected(NodeContext context, TopologyHandle handle, string queueName)
        {
            ForEach(x => handle = x.ConsumerConnected(context, handle, queueName));

            return handle;
        }
    }
}
