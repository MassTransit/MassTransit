namespace MassTransit.InMemoryTransport.Fabric
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IInMemoryExchange :
        IMessageSink<InMemoryTransportMessage>,
        IMessageSource<InMemoryTransportMessage>
    {
        string Name { get; }
        Task Send(InMemoryTransportMessage message, CancellationToken cancellationToken);
    }
}
