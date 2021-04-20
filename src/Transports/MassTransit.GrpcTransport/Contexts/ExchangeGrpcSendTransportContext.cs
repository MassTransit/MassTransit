namespace MassTransit.GrpcTransport.Contexts
{
    using Context;
    using Fabric;
    using MassTransit.Configuration;


    public class ExchangeGrpcSendTransportContext :
        BaseSendTransportContext,
        GrpcSendTransportContext
    {
        public ExchangeGrpcSendTransportContext(IHostConfiguration hostConfiguration, IGrpcExchange exchange)
            : base(hostConfiguration)
        {
            Exchange = exchange;
        }

        public IGrpcExchange Exchange { get; }
    }
}