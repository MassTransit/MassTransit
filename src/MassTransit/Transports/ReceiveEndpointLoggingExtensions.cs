namespace MassTransit.Transports
{
    using System;
    using System.Runtime.CompilerServices;
    using Logging;
    using Microsoft.Extensions.Logging;


    public static class ReceiveEndpointLoggingExtensions
    {
        static readonly LogMessage<Uri, Guid?, string, string, TimeSpan> _logConsumed = LogContext.DefineMessage<Uri, Guid?, string, string, TimeSpan>(
            LogLevel.Debug, "RECEIVE {InputAddress} {MessageId} {MessageType} {ConsumerType}({Duration})");

        static readonly LogMessage<Uri, Guid?, string, string, TimeSpan> _logConsumeFault = LogContext.Define<Uri, Guid?, string, string, TimeSpan>(
            LogLevel.Error, "R-FAULT {InputAddress} {MessageId} {MessageType} {ConsumerType}({Duration})");

        static readonly LogMessage<Uri, string, string, string> _logMoved = LogContext.DefineMessage<Uri, string, string, string>(LogLevel.Information,
            "MOVE {InputAddress} {MessageId} {DestinationAddress} {Reason}");

        static readonly LogMessage<Uri, string, TimeSpan> _logReceiveFault = LogContext.Define<Uri, string, TimeSpan>(LogLevel.Error,
            "R-FAULT {InputAddress} {MessageId} {Duration}");

        static readonly LogMessage<Uri, Guid?, string> _logSent = LogContext.DefineMessage<Uri, Guid?, string>(LogLevel.Debug,
            "SEND {DestinationAddress} {MessageId} {MessageType}");

        static readonly LogMessage<Uri, Guid?, string> _logSendFault = LogContext.Define<Uri, Guid?, string>(LogLevel.Error,
            "S-FAULT {DestinationAddress} {MessageId} {MessageType}");

        static readonly LogMessage<Uri, string> _logSkipped = LogContext.DefineMessage<Uri, string>(LogLevel.Debug,
            "SKIP {InputAddress} {MessageId}");

        static readonly LogMessage<Uri, Guid?, string> _logRetry = LogContext.Define<Uri, Guid?, string>(LogLevel.Warning,
            "R-RETRY {InputAddress} {MessageId} {MessageType}");

        static readonly LogMessage<Uri, string> _logFault = LogContext.Define<Uri, string>(LogLevel.Error,
            "T-FAULT {InputAddress} {MessageId}");

        static readonly LogMessage<Uri, Guid?, string, DateTime, Guid?> _logScheduled = LogContext.DefineMessage<Uri, Guid?, string, DateTime, Guid?>(
            LogLevel.Debug, "SCHED {DestinationAddress} {MessageId} {MessageType} {DeliveryTime:G} {Token}");

        /// <summary>
        /// Log a skipped message that was moved to the dead-letter queue
        /// </summary>
        /// <param name="context"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogSkipped(this ReceiveContext context)
        {
            _logSkipped(context.InputAddress, GetMessageId(context));
        }

        /// <summary>
        /// Log a moved message from one endpoint to the destination endpoint address
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destination"></param>
        /// <param name="reason"> </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogMoved(this ReceiveContext context, string destination, string reason)
        {
            _logMoved(context.InputAddress, GetMessageId(context), destination, reason);
        }

        /// <summary>
        /// Log a consumed message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType"></param>
        /// <typeparam name="T"></typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogConsumed<T>(this ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            _logConsumed(context.ReceiveContext.InputAddress, context.MessageId, TypeCache<T>.ShortName, consumerType, duration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogFaulted<T>(this ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            _logConsumeFault(context.ReceiveContext.InputAddress, context.MessageId, TypeCache<T>.ShortName, consumerType, duration, exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogFaulted(this ReceiveContext context, Exception exception)
        {
            _logReceiveFault(context.InputAddress, GetMessageId(context), context.ElapsedTime, exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogTransportFaulted(this ReceiveContext context, Exception exception)
        {
            _logFault(context.InputAddress, GetMessageId(context), exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogRetry(this ConsumeContext context, Exception exception)
        {
            _logRetry(context.ReceiveContext.InputAddress, context.MessageId, TypeCache.GetShortName(context.GetType()), exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogRetry<TContext>(this TContext context, Exception exception)
            where TContext : class, ConsumeContext
        {
            _logRetry(context.ReceiveContext.InputAddress, context.MessageId, TypeCache<TContext>.ShortName, exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogFaulted<T>(this SendContext<T> context, Exception exception)
            where T : class
        {
            _logSendFault(context.DestinationAddress, context.MessageId, TypeCache<T>.ShortName, exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogSent<T>(this SendContext<T> context)
            where T : class
        {
            _logSent(context.DestinationAddress, context.MessageId, TypeCache<T>.ShortName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogScheduled<T>(this SendContext<T> context, DateTime deliveryTime)
            where T : class
        {
            _logScheduled(context.DestinationAddress, context.MessageId, TypeCache<T>.ShortName, deliveryTime, context.ScheduledMessageId);
        }

        static string GetMessageId(ReceiveContext context)
        {
            try
            {
                return context.GetMessageId()?.ToString() ?? context.TransportHeaders.Get<string>(MessageHeaders.TransportMessageId);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
