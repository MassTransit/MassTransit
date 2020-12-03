namespace MassTransit.RabbitMqTransport
{
    using Context;
    using Microsoft.Extensions.Logging;
    using Topology.Entities;


    public static class RabbitMqLogMessages
    {
        public static readonly LogMessage<ExchangeToExchangeBinding> BindToExchange = LogContext.Define<ExchangeToExchangeBinding>(LogLevel.Debug,
            "Bind exchange: {ExchangeBinding}");

        public static readonly LogMessage<ExchangeToQueueBinding> BindToQueue = LogContext.Define<ExchangeToQueueBinding>(LogLevel.Debug,
            "Bind queue: {QueueBinding}");

        public static readonly LogMessage<Exchange> DeclareExchange = LogContext.Define<Exchange>(LogLevel.Debug,
            "Declare exchange: {Exchange}");

        public static readonly LogMessage<Queue> DeclareQueue = LogContext.Define<Queue>(LogLevel.Debug,
            "Declare queue: {Queue}");

        public static readonly LogMessage<ushort> PrefetchCount = LogContext.Define<ushort>(LogLevel.Debug,
            "Set Prefetch Count: {PrefetchCount}");
    }
}
