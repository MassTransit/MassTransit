namespace MassTransit.Transports.Fabric
{
    using Util;


    public class MessageFabricObservable<TContext> :
        Connectable<IMessageFabricObserver<TContext>>,
        IMessageFabricObserver<TContext>
        where TContext : class
    {
        public void ExchangeDeclared(TContext context, string name, ExchangeType exchangeType)
        {
            ForEach(x => x.ExchangeDeclared(context, name, exchangeType));
        }

        public void ExchangeBindingCreated(TContext context, string source, string destination, string routingKey)
        {
            ForEach(x => x.ExchangeBindingCreated(context, source, destination, routingKey));
        }

        public void QueueDeclared(TContext context, string name)
        {
            ForEach(x => x.QueueDeclared(context, name));
        }

        public void QueueBindingCreated(TContext context, string source, string destination)
        {
            ForEach(x => x.QueueBindingCreated(context, source, destination));
        }

        public TopologyHandle ConsumerConnected(TContext context, TopologyHandle handle, string queueName)
        {
            ForEach(x => handle = x.ConsumerConnected(context, handle, queueName));

            return handle;
        }
    }
}
