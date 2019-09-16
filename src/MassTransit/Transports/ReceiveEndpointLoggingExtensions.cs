namespace MassTransit.Transports
{
    using System;
    using Context;
    using Internals.Extensions;
    using Metadata;
    using Util;


    public static class ReceiveEndpointLoggingExtensions
    {
        /// <summary>
        /// Log that a message was skipped, and moved to the dead letter queue
        /// </summary>
        /// <param name="context"></param>
        public static void LogSkipped(this ReceiveContext context)
        {
            LogContext.Current?.Messages.Debug?.Log("SKIP {InputAddress} {MessageId}", context.InputAddress, GetMessageId(context));
        }

        /// <summary>
        /// Log that a message was moved from one endpoint to the destination endpoint address
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destination"></param>
        /// <param name="reason"> </param>
        public static void LogMoved(this ReceiveContext context, string destination, string reason)
        {
            LogContext.Current?.Messages.Info?.Log("MOVE {InputAddress} {MessageId} {DestinationAddress} {Reason}", context.InputAddress,
                GetMessageId(context), destination, reason);
        }

        public static void LogConsumed<T>(this ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            LogContext.Current?.Messages.Debug?.Log("RECEIVE {InputAddress} {MessageId} {MessageType} {ConsumerType}({Duration})",
                context.ReceiveContext.InputAddress, context.MessageId, TypeMetadataCache<T>.ShortName, consumerType, duration.ToFriendlyString());
        }

        public static void LogFaulted<T>(this ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            LogContext.Current?.Messages.Error?.Log("R-FAULT {InputAddress} {MessageId} {MessageType} {ConsumerType}({Duration}) {Exception}",
                context.ReceiveContext.InputAddress, context.MessageId, TypeMetadataCache<T>.ShortName, consumerType, duration.ToFriendlyString(),
                GetFaultMessage(exception));
        }

        public static void LogFaulted(this ReceiveContext context, Exception exception)
        {
            LogContext.Current?.Messages.Error?.Log("R-FAULT {InputAddress} {MessageId} {Exception}", context.InputAddress, GetMessageId(context),
                GetFaultMessage(exception));
        }

        public static void LogRetry(this ConsumeContext context, Exception exception)
        {
            LogContext.Current?.Messages.Warning?.Log("R-RETRY {InputAddress} {MessageId} {Exception}", context.ReceiveContext.InputAddress,
                context.MessageId, GetFaultMessage(exception));
        }

        public static void LogRetry<T>(this ConsumeContext<T> context, Exception exception)
            where T : class
        {
            LogContext.Current?.Messages.Warning?.Log("R-RETRY {InputAddress} {MessageId} {MessageType} {Exception}", context.ReceiveContext.InputAddress,
                context.MessageId, TypeMetadataCache<T>.ShortName, GetFaultMessage(exception));
        }

        public static void LogFaulted<T>(this SendContext<T> context, Exception exception)
            where T : class
        {
            LogContext.Current?.Messages.Error?.Log("S-FAULT {DestinationAddress} {MessageId} {MessageType} {Exception}", context.DestinationAddress,
                context.MessageId, TypeMetadataCache<T>.ShortName, GetFaultMessage(exception));
        }

        public static void LogSent<T>(this SendContext<T> context)
            where T : class
        {
            LogContext.Current?.Messages.Debug?.Log("SEND {DestinationAddress} {MessageId} {MessageType}", context.DestinationAddress, context.MessageId,
                TypeMetadataCache<T>.ShortName);
        }

        public static void LogScheduled<T>(this SendContext<T> context, DateTime deliveryTime)
            where T : class
        {
            LogContext.Current?.Messages.Debug?.Log("SCHED {DestinationAddress} {MessageId} {MessageType} {DeliveryTime:G} {Token}", context.DestinationAddress,
                context.MessageId, TypeMetadataCache<T>.ShortName, deliveryTime, context.ScheduledMessageId?.ToString("D"));
        }

        static string GetMessageId(ReceiveContext context)
        {
            return context.TransportHeaders.Get("MessageId", "N/A");
        }

        static string GetFaultMessage(Exception exception)
        {
            var baseException = exception.GetBaseException() ?? exception;

            return ExceptionUtil.GetMessage(baseException);
        }
    }
}
