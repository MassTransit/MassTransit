namespace MassTransit.GrpcTransport
{
    using Fabric;
    using MassTransit.Configuration;
    using Transports;


    public class ExchangeGrpcSendTransportContext :
        BaseSendTransportContext,
        GrpcSendTransportContext
    {
        public ExchangeGrpcSendTransportContext(IHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext, IMessageExchange exchange)
            : base(hostConfiguration, receiveEndpointContext.Serialization)
        {
            Exchange = exchange;
        }

        public IMessageExchange Exchange { get; }
    }
}
