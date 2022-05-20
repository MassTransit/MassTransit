namespace MassTransit.GrpcTransport
{
    using Fabric;
    using MassTransit.Configuration;
    using Transports;
    using Transports.Fabric;


    public class ExchangeGrpcSendTransportContext :
        BaseSendTransportContext,
        GrpcSendTransportContext
    {
        public ExchangeGrpcSendTransportContext(IHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
            IMessageExchange<GrpcTransportMessage> exchange)
            : base(hostConfiguration, receiveEndpointContext.Serialization)
        {
            Exchange = exchange;
        }

        public IMessageExchange<GrpcTransportMessage> Exchange { get; }
        public override string EntityName => Exchange.Name;
        public override string ActivitySystem => "grpc";
    }
}
