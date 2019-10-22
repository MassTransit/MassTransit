namespace MassTransit.Transports.InMemory.Contexts
{
    using Context;
    using Fabric;


    public interface InMemorySendTransportContext :
        SendTransportContext
    {
        IInMemoryExchange Exchange { get; }
    }
}
