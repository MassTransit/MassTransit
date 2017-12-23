namespace MassTransit.Transports.InMemory.Fabric
{
    using System.Threading.Tasks;


    public interface IInMemoryExchange :
        IMessageSink<InMemoryTransportMessage>,
        IMessageSource<InMemoryTransportMessage>
    {
        Task Send(InMemoryTransportMessage message);
        string Name { get; }
    }
}