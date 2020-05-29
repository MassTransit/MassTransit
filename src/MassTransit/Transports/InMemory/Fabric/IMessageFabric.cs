namespace MassTransit.Transports.InMemory.Fabric
{
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IMessageFabric :
        IProbeSite
    {
        int ConcurrencyLimit { set; }
        Task Send(string exchangeName, InMemoryTransportMessage message);

        void ExchangeDeclare(string name);
        void QueueDeclare(string name, int concurrencyLimit);
        void ExchangeBind(string source, string destination);
        void QueueBind(string source, string destination);
        IInMemoryQueue GetQueue(string name);
        IInMemoryExchange GetExchange(string name);
    }
}
