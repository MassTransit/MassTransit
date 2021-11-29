namespace MassTransit.GrpcTransport
{
    using System;
    using System.Runtime.CompilerServices;
    using Contracts;
    using Logging;
    using Microsoft.Extensions.Logging;


    public static class GrpcLogExtensions
    {
        static readonly LogMessage<Uri, string, TransportMessage.ContentOneofCase> _logSent =
            LogContext.Define<Uri, string, TransportMessage.ContentOneofCase>(LogLevel.Debug, "GRPC-SEND {NodeAddress} {MessageId} {MessageType}");

        static readonly LogMessage<Uri, string, TransportMessage.ContentOneofCase> _logReceived =
            LogContext.Define<Uri, string, TransportMessage.ContentOneofCase>(LogLevel.Debug, "GRPC-RECV {NodeAddress} {MessageId} {MessageType}");

        static readonly LogMessage<Uri> _logDisconnect =
            LogContext.Define<Uri>(LogLevel.Debug, "GRPC-PART {NodeAddress}");

        static readonly LogMessage<Uri, string> _logExchange =
            LogContext.Define<Uri, string>(LogLevel.Debug, "TOPOLOGY Exchange {NodeAddress} {Exchange}");

        static readonly LogMessage<Uri, string, string> _logExchangeBind =
            LogContext.Define<Uri, string, string>(LogLevel.Debug, "TOPOLOGY ExchangeBind {NodeAddress} {Source} -> {Destination}");

        static readonly LogMessage<Uri, string> _logQueue =
            LogContext.Define<Uri, string>(LogLevel.Debug, "TOPOLOGY Queue {NodeAddress} {Queue}");

        static readonly LogMessage<Uri, string, string> _logQueueBind =
            LogContext.Define<Uri, string, string>(LogLevel.Debug, "TOPOLOGY QueueBind {NodeAddress} {Source} -> {Destination}");

        static readonly LogMessage<Uri, string> _logConsumer =
            LogContext.Define<Uri, string>(LogLevel.Debug, "TOPOLOGY Consumer {NodeAddress} {Queue}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogSent(this NodeContext context, TransportMessage message)
        {
            // _logSent(context.NodeAddress, message.MessageId, message.ContentCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogReceived(this NodeContext context, TransportMessage message)
        {
            // _logReceived(context.NodeAddress, message.MessageId, message.ContentCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogDisconnect(this NodeContext context)
        {
            _logDisconnect(context.NodeAddress);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogTopology(this NodeContext context, Exchange exchange, ExchangeType exchangeType)
        {
            // _logExchange(context.NodeAddress, exchange.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogTopology(this NodeContext context, ExchangeBind exchangeBind)
        {
            // _logExchangeBind(context.NodeAddress, exchangeBind.Source, exchangeBind.Destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogTopology(this NodeContext context, Queue queue)
        {
            // _logQueue(context.NodeAddress, queue.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogTopology(this NodeContext context, QueueBind queueBind)
        {
            // _logQueueBind(context.NodeAddress, queueBind.Source, queueBind.Destination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogTopology(this NodeContext context, Receiver receiver)
        {
            // _logConsumer(context.NodeAddress, consumer.QueueName);
        }
    }
}
