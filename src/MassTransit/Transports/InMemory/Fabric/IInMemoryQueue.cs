namespace MassTransit.Transports.InMemory.Fabric
{
    using GreenPipes;


    public interface IInMemoryQueue :
        IMessageSink<InMemoryTransportMessage>
    {
        ConnectHandle ConnectConsumer(IInMemoryQueueConsumer consumer);
    }
}
