namespace MassTransit.InMemoryTransport
{
    using MassTransit.Configuration;
    using Transports;
    using Transports.Fabric;


    public class ExchangeInMemorySendTransportContext :
        BaseSendTransportContext,
        InMemorySendTransportContext
    {
        public ExchangeInMemorySendTransportContext(IHostConfiguration hostConfiguration, ReceiveEndpointContext context,
            IMessageExchange<InMemoryTransportMessage> exchange)
            : base(hostConfiguration, context.Serialization)
        {
            Exchange = exchange;
        }

        public override string EntityName => Exchange.Name;
        public override string ActivitySystem => "in-memory";

        public IMessageExchange<InMemoryTransportMessage> Exchange { get; }
    }
}
