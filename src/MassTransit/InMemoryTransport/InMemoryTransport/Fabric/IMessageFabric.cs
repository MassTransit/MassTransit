namespace MassTransit.InMemoryTransport.Fabric
{
    using System;


    public interface IMessageFabric :
        IProbeSite,
        IAsyncDisposable
    {
        void ExchangeDeclare(string name);
        void QueueDeclare(string name, int concurrencyLimit);
        void ExchangeBind(string source, string destination);
        void QueueBind(string source, string destination);
        IInMemoryQueue GetQueue(string name);
        IInMemoryExchange GetExchange(string name);
    }
}
