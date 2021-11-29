namespace MassTransit.InMemoryTransport.Fabric
{
    using System;


    public interface IInMemoryQueue :
        IMessageSink<InMemoryTransportMessage>,
        IAsyncDisposable
    {
        ConnectHandle ConnectConsumer(IInMemoryQueueConsumer consumer);
    }
}
