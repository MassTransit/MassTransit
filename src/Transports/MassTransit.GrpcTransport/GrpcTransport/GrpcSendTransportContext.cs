namespace MassTransit.GrpcTransport
{
    using Fabric;
    using Transports;
    using Transports.Fabric;


    public interface GrpcSendTransportContext :
        SendTransportContext
    {
        IMessageExchange<GrpcTransportMessage> Exchange { get; }
    }
}
