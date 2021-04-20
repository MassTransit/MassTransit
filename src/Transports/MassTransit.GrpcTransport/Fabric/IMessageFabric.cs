namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using Contexts;
    using Contracts;
    using GreenPipes;


    public interface IMessageFabric :
        IMessageFabricObserverConnector,
        IProbeSite,
        IAsyncDisposable
    {
        void ExchangeDeclare(NodeContext context, string name, ExchangeType exchangeType);

        void ExchangeBind(NodeContext context, string source, string destination, string routingKey);

        void QueueDeclare(NodeContext context, string name);

        void QueueBind(NodeContext context, string source, string destination);

        IGrpcExchange GetExchange(NodeContext context, string name, ExchangeType exchangeType = ExchangeType.FanOut);

        IGrpcQueue GetQueue(NodeContext context, string name);
    }
}
