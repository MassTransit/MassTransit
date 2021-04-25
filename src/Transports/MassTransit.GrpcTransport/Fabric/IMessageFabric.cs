namespace MassTransit.GrpcTransport.Fabric
{
    using Contexts;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Agents;


    public interface IMessageFabric :
        IMessageFabricObserverConnector,
        IAgent,
        IProbeSite
    {
        void ExchangeDeclare(NodeContext context, string name, ExchangeType exchangeType);

        void ExchangeBind(NodeContext context, string source, string destination, string routingKey);

        void QueueDeclare(NodeContext context, string name);

        void QueueBind(NodeContext context, string source, string destination);

        IMessageExchange GetExchange(NodeContext context, string name, ExchangeType exchangeType = ExchangeType.FanOut);

        IMessageQueue GetQueue(NodeContext context, string name);
    }
}
