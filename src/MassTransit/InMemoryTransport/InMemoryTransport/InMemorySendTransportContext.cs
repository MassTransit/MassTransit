namespace MassTransit.InMemoryTransport
{
    using Transports;
    using Transports.Fabric;


    public interface InMemorySendTransportContext :
        SendTransportContext
    {
        IMessageExchange<InMemoryTransportMessage> Exchange { get; }
    }
}
