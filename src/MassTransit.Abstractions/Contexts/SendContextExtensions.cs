namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Metadata;
    using Transports;
    using Util;


    public static class SendContextExtensions
    {
        /// <summary>
        /// Set the host headers on the SendContext (for error, dead-letter, etc.)
        /// </summary>
        /// <param name="headers"></param>
        public static void SetHostHeaders(this SendHeaders headers)
        {
            headers.Set(MessageHeaders.Host.MachineName, HostMetadataCache.Host.MachineName);
            headers.Set(MessageHeaders.Host.ProcessName, HostMetadataCache.Host.ProcessName);
            headers.Set(MessageHeaders.Host.ProcessId, HostMetadataCache.Host.ProcessId.ToString("F0"));
            headers.Set(MessageHeaders.Host.Assembly, HostMetadataCache.Host.Assembly);
            headers.Set(MessageHeaders.Host.AssemblyVersion, HostMetadataCache.Host.AssemblyVersion);
            headers.Set(MessageHeaders.Host.MassTransitVersion, HostMetadataCache.Host.MassTransitVersion);
            headers.Set(MessageHeaders.Host.FrameworkVersion, HostMetadataCache.Host.FrameworkVersion);
            headers.Set(MessageHeaders.Host.OperatingSystemVersion, HostMetadataCache.Host.OperatingSystemVersion);
        }

        /// <summary>
        /// Set the host headers on the SendContext (for error, dead-letter, etc.)
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="dictionary"></param>
        public static void SetHostHeaders<T>(this ITransportSetHeaderAdapter<T> adapter, IDictionary<string, T> dictionary)
        {
            adapter.Set(dictionary, MessageHeaders.Host.MachineName, HostMetadataCache.Host.MachineName);
            adapter.Set(dictionary, MessageHeaders.Host.ProcessName, HostMetadataCache.Host.ProcessName);
            adapter.Set(dictionary, MessageHeaders.Host.ProcessId, HostMetadataCache.Host.ProcessId);
            adapter.Set(dictionary, MessageHeaders.Host.Assembly, HostMetadataCache.Host.Assembly);
            adapter.Set(dictionary, MessageHeaders.Host.AssemblyVersion, HostMetadataCache.Host.AssemblyVersion);
            adapter.Set(dictionary, MessageHeaders.Host.MassTransitVersion, HostMetadataCache.Host.MassTransitVersion);
            adapter.Set(dictionary, MessageHeaders.Host.FrameworkVersion, HostMetadataCache.Host.FrameworkVersion);
            adapter.Set(dictionary, MessageHeaders.Host.OperatingSystemVersion, HostMetadataCache.Host.OperatingSystemVersion);
        }

        /// <summary>
        /// Set the host headers on the SendContext (for error, dead-letter, etc.)
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="exceptionContext"></param>
        public static void SetExceptionHeaders(this SendHeaders headers, ExceptionReceiveContext exceptionContext)
        {
            var exception = exceptionContext.Exception.GetBaseException() ?? exceptionContext.Exception;

            var exceptionMessage = ExceptionUtil.GetMessage(exception);

            headers.Set(MessageHeaders.Reason, "fault");

            headers.Set(MessageHeaders.FaultExceptionType, TypeCache.GetShortName(exception.GetType()));
            headers.Set(MessageHeaders.FaultMessage, exceptionMessage);
            headers.Set(MessageHeaders.FaultTimestamp, exceptionContext.ExceptionTimestamp.ToString("O"));
            headers.Set(MessageHeaders.FaultStackTrace, ExceptionUtil.GetStackTrace(exception));

            if (exceptionContext.TryGetPayload(out ConsumerFaultContext? info))
            {
                headers.Set(MessageHeaders.FaultConsumerType, info!.ConsumerType);
                headers.Set(MessageHeaders.FaultMessageType, info.MessageType);
            }

            if (exceptionContext.TryGetPayload(out RetryContext? retryContext) && retryContext!.RetryCount > 0)
                headers.Set(MessageHeaders.FaultRetryCount, retryContext.RetryCount);
        }

        /// <summary>
        /// Set the host headers on the SendContext (for error, dead-letter, etc.)
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="headers"></param>
        /// <param name="exceptionContext"></param>
        public static void SetExceptionHeaders<T>(this ITransportSetHeaderAdapter<T> adapter, IDictionary<string, T> headers, ExceptionReceiveContext
            exceptionContext)
        {
            var exception = exceptionContext.Exception.GetBaseException() ?? exceptionContext.Exception;

            var exceptionMessage = ExceptionUtil.GetMessage(exception);

            adapter.Set(headers, MessageHeaders.Reason, "fault");

            adapter.Set(headers, MessageHeaders.FaultExceptionType, TypeCache.GetShortName(exception.GetType()));
            adapter.Set(headers, MessageHeaders.FaultMessage, exceptionMessage);
            adapter.Set(headers, MessageHeaders.FaultTimestamp, exceptionContext.ExceptionTimestamp);
            adapter.Set(headers, MessageHeaders.FaultStackTrace, ExceptionUtil.GetStackTrace(exception));

            if (exceptionContext.TryGetPayload(out ConsumerFaultContext? info))
            {
                adapter.Set(headers, MessageHeaders.FaultConsumerType, info!.ConsumerType);
                adapter.Set(headers, MessageHeaders.FaultMessageType, info.MessageType);
            }

            if (exceptionContext.TryGetPayload(out RetryContext? retryContext) && retryContext!.RetryCount > 0)
                adapter.Set(headers, MessageHeaders.FaultRetryCount, retryContext.RetryCount);
        }

        /// <summary>
        /// Transfer the header information from the ConsumeContext to the SendContext, including any non-MT headers.
        /// </summary>
        /// <param name="sendContext"></param>
        /// <param name="consumeContext"></param>
        public static void TransferConsumeContextHeaders(this SendContext sendContext, ConsumeContext consumeContext)
        {
            sendContext.GetOrAddPayload(() => consumeContext);

            sendContext.SourceAddress = consumeContext.ReceiveContext.InputAddress;

            if (consumeContext.ConversationId.HasValue)
                sendContext.ConversationId = consumeContext.ConversationId;

            if (consumeContext.CorrelationId.HasValue)
                sendContext.InitiatorId = consumeContext.CorrelationId;
            else if (consumeContext.RequestId.HasValue)
                sendContext.InitiatorId = consumeContext.RequestId;

            var sendHeaders = sendContext.Headers;

            foreach (KeyValuePair<string, object> header in consumeContext.Headers.GetAll())
            {
                if (header.Key.StartsWith("MT-"))
                    continue;

                sendHeaders.Set(header.Key, header.Value, false);
            }
        }

        public static void ApplyRedeliveryOptions(this SendContext sendContext, ConsumeContext consumeContext, RedeliveryOptions options)
        {
            if (options.HasFlag(RedeliveryOptions.ReplaceMessageId))
                sendContext.ReplaceMessageId(consumeContext);

            sendContext.Headers.Set(MessageHeaders.RedeliveryCount, consumeContext.GetRedeliveryCount() + 1);
        }

        /// <summary>
        /// Generate a new MessageId, storing the original MessageId in the OriginalMessageId header (unless it already exists)
        /// </summary>
        /// <param name="sendContext"></param>
        /// <param name="consumeContext"></param>
        /// <returns></returns>
        public static SendContext ReplaceMessageId(this SendContext sendContext, ConsumeContext consumeContext)
        {
            sendContext.Headers.Set(MessageHeaders.OriginalMessageId,
                consumeContext.TryGetHeader(MessageHeaders.OriginalMessageId, out Guid? originalMessageId)
                    ? originalMessageId.ToString()
                    : consumeContext.MessageId.ToString());

            sendContext.MessageId = NewId.NextGuid();

            return sendContext;
        }

        /// <summary>
        /// Sets the ConversationId to a new value, starting a new conversation. If a message was being consumed, and the
        /// ConversationId was present, that value is stored in an MT-InitiatingConversationId header.
        /// </summary>
        /// <param name="context">The send context</param>
        /// <returns></returns>
        public static SendContext StartNewConversation(this SendContext context)
        {
            return StartNewConversation(context, NewId.NextGuid());
        }

        /// <summary>
        /// Sets the ConversationId to a new value, starting a new conversation. If a message was being consumed, and the
        /// ConversationId was present, that value is stored in an MT-InitiatingConversationId header.
        /// </summary>
        /// <param name="context">The send context</param>
        /// <param name="conversationId">The new ConversationId</param>
        /// <returns></returns>
        public static SendContext StartNewConversation(this SendContext context, Guid conversationId)
        {
            if (context.ConversationId.HasValue)
                context.Headers.Set(MessageHeaders.InitiatingConversationId, context.ConversationId.Value.ToString());

            context.ConversationId = conversationId;

            return context;
        }
    }
}
