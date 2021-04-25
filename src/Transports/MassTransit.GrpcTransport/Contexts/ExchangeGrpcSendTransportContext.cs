namespace MassTransit.GrpcTransport.Contexts
{
    using Context;
    using Fabric;
    using MassTransit.Configuration;


    public class ExchangeGrpcSendTransportContext :
        BaseSendTransportContext,
        GrpcSendTransportContext
    {
        public ExchangeGrpcSendTransportContext(IHostConfiguration hostConfiguration, IMessageExchange exchange)
            : base(hostConfiguration)
        {
            Exchange = exchange;
        }

        public IMessageExchange Exchange { get; }
    }
}