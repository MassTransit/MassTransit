namespace MassTransit.Transports.InMemory.Fabric
{
    using System.Threading.Tasks;


    public interface IInMemoryExchange :
        IMessageSink<InMemoryTransportMessage>,
        IMessageSource<InMemoryTransportMessage>
    {
        string Name { get; }
        Task Send(InMemoryTransportMessage message);
    }
}
