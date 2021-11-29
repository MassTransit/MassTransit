namespace MassTransit.InMemoryTransport
{
    using Context;
    using Fabric;
    using Transports;


    public interface InMemorySendTransportContext :
        SendTransportContext
    {
        IInMemoryExchange Exchange { get; }
    }
}
