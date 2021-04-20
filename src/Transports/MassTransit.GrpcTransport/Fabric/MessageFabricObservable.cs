namespace MassTransit.GrpcTransport.Fabric
{
    using Contexts;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Util;


    public class MessageFabricObservable :
        Connectable<IMessageFabricObserver>,
        IMessageFabricObserver
    {
        public void ExchangeDeclared(NodeContext context, string name, ExchangeType exchangeType)
        {
            All(x =>
            {
                x.ExchangeDeclared(context, name, exchangeType);

                return true;
            });
        }

        public void ExchangeBindingCreated(NodeContext context, string source, string destination, string routingKey)
        {
            All(x =>
            {
                x.ExchangeBindingCreated(context, source, destination, routingKey);

                return true;
            });
        }

        public void QueueDeclared(NodeContext context, string name)
        {
            All(x =>
            {
                x.QueueDeclared(context, name);

                return true;
            });
        }

        public void QueueBindingCreated(NodeContext context, string source, string destination)
        {
            All(x =>
            {
                x.QueueBindingCreated(context, source, destination);

                return true;
            });
        }

        public ConnectHandle ConsumerConnected(NodeContext context, ConnectHandle handle, string queueName)
        {
            All(x =>
            {
                handle = x.ConsumerConnected(context, handle, queueName);

                return true;
            });

            return handle;
        }
    }
}
