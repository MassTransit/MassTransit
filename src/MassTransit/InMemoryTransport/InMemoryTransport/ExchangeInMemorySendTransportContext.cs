namespace MassTransit.InMemoryTransport
{
    using Fabric;
    using MassTransit.Configuration;
    using Transports;


    public class ExchangeInMemorySendTransportContext :
        BaseSendTransportContext,
        InMemorySendTransportContext
    {
        public ExchangeInMemorySendTransportContext(IHostConfiguration hostConfiguration, ReceiveEndpointContext context, IInMemoryExchange exchange)
            : base(hostConfiguration, context.Serialization)
        {
            Exchange = exchange;
        }

        public IInMemoryExchange Exchange { get; }
        public override string EntityName => Exchange.Name;
    }
}
