namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using GreenPipes;


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
