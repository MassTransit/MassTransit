namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using GreenPipes;


    public interface IInMemoryQueue :
        IMessageSink<InMemoryTransportMessage>,
        IAsyncDisposable
    {
        ConnectHandle ConnectConsumer(IInMemoryQueueConsumer consumer);
    }
}
