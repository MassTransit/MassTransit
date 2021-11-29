namespace MassTransit.RabbitMqTransport
{
    using Logging;
    using Microsoft.Extensions.Logging;
    using Topology;


    public static class RabbitMqLogMessages
    {
        public static readonly LogMessage<ExchangeToExchangeBinding> BindToExchange = LogContext.Define<ExchangeToExchangeBinding>(LogLevel.Debug,
            "Bind exchange: {ExchangeBinding}");

        public static readonly LogMessage<ExchangeToQueueBinding> BindToQueue = LogContext.Define<ExchangeToQueueBinding>(LogLevel.Debug,
            "Bind queue: {QueueBinding}");

        public static readonly LogMessage<Exchange> DeclareExchange = LogContext.Define<Exchange>(LogLevel.Debug,
            "Declare exchange: {Exchange}");

        public static readonly LogMessage<Queue, uint, uint> DeclareQueue = LogContext.Define<Queue, uint, uint>(LogLevel.Debug,
            "Declare queue: {Queue}, consumer-count: {ConsumerCount} message-count: {MessageCount}");

        public static readonly LogMessage<ushort> PrefetchCount = LogContext.Define<ushort>(LogLevel.Debug,
            "Set Prefetch Count: {PrefetchCount}");
    }
}
