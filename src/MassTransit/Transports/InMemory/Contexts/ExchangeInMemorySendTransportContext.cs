namespace MassTransit.Transports.InMemory.Contexts
{
    using Context;
    using Fabric;


    public class ExchangeInMemorySendTransportContext :
        BaseSendTransportContext,
        InMemorySendTransportContext
    {
        public ExchangeInMemorySendTransportContext(IInMemoryExchange exchange, ILogContext logContext)
            : base(logContext)
        {
            Exchange = exchange;
        }

        public IInMemoryExchange Exchange { get; }
    }
}
