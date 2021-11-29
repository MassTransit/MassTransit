namespace MassTransit.GrpcTransport
{
    using Fabric;
    using Transports;


    public interface GrpcSendTransportContext :
        SendTransportContext
    {
        IMessageExchange Exchange { get; }
    }
}