namespace MassTransit.GrpcTransport.Contexts
{
    using Context;
    using Fabric;


    public interface GrpcSendTransportContext :
        SendTransportContext
    {
        IMessageExchange Exchange { get; }
    }
}