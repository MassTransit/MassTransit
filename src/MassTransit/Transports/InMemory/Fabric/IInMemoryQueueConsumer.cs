namespace MassTransit.Transports.InMemory.Fabric
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IInMemoryQueueConsumer
    {
        Task Consume(InMemoryTransportMessage message, CancellationToken cancellationToken);
    }
}
