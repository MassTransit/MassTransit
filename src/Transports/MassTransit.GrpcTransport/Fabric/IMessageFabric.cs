namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using Contexts;
    using GreenPipes;


    public interface IMessageFabric :
        IMessageFabricObserverConnector,
        IProbeSite,
        IAsyncDisposable
    {
        void ExchangeDeclare(NodeContext context, string name);
        void QueueDeclare(NodeContext context, string name, int? concurrencyLimit = default);
        void ExchangeBind(NodeContext context, string source, string destination, string routingKey = default);
        void QueueBind(NodeContext context, string source, string destination, string routingKey = default);
        IGrpcQueue GetQueue(NodeContext context, string name);
        IGrpcExchange GetExchange(NodeContext context, string name);
    }
}