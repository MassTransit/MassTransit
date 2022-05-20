namespace MassTransit.Transports.Fabric
{
    public interface IMessageFabric<TContext, T> :
        IMessageFabricObserverConnector<TContext>,
        IAgent,
        IProbeSite
        where T : class
        where TContext : class
    {
        void ExchangeDeclare(TContext context, string name, ExchangeType exchangeType);

        void ExchangeBind(TContext context, string source, string destination, string routingKey);

        void QueueDeclare(TContext context, string name);

        void QueueBind(TContext context, string source, string destination);

        IMessageExchange<T> GetExchange(TContext context, string name, ExchangeType exchangeType = ExchangeType.FanOut);

        IMessageQueue<TContext, T> GetQueue(TContext context, string name);
    }
}
