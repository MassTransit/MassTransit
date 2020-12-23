namespace MassTransit.Transports.InMemory.Contexts
{
    using Context;
    using Fabric;
    using MassTransit.Configuration;


    public class ExchangeInMemorySendTransportContext :
        BaseSendTransportContext,
        InMemorySendTransportContext
    {
        public ExchangeInMemorySendTransportContext(IHostConfiguration hostConfiguration, IInMemoryExchange exchange)
            : base(hostConfiguration)
        {
            Exchange = exchange;
        }

        public IInMemoryExchange Exchange { get; }
    }
}
