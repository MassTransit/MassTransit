namespace MassTransit
{
    using System;


    public static class ReceiveContextExtensions
    {
        /// <summary>
        /// Returns the messageId from the transport header, if available
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Guid? GetMessageId(this ReceiveContext context)
        {
            return context.TransportHeaders.GetHeaderId(MessageHeaders.MessageId);
        }

        /// <summary>
        /// Returns the messageId from the transport header, if available
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Guid GetMessageId(this ReceiveContext context, Guid defaultValue)
        {
            return context.TransportHeaders.GetHeaderId(MessageHeaders.MessageId, defaultValue);
        }

        /// <summary>
        /// Returns the CorrelationId from the transport header, if available
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Guid? GetCorrelationId(this ReceiveContext context)
        {
            return context.TransportHeaders.GetHeaderId(MessageHeaders.CorrelationId);
        }

        /// <summary>
        /// Returns the ConversationId from the transport header, if available
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Guid? GetConversationId(this ReceiveContext context)
        {
            return context.TransportHeaders.GetHeaderId(MessageHeaders.ConversationId);
        }

        /// <summary>
        /// Returns the RequestId from the transport header, if available
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Guid? GetRequestId(this ReceiveContext context)
        {
            return context.TransportHeaders.GetHeaderId(MessageHeaders.RequestId);
        }

        /// <summary>
        /// Returns the InitiatorId from the transport header, if available
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Guid? GetInitiatorId(this ReceiveContext context)
        {
            return context.TransportHeaders.GetHeaderId(MessageHeaders.InitiatorId);
        }

        public static Guid GetHeaderId(this Headers headers, string key, Guid defaultValue)
        {
            return GetHeaderId(headers, key) ?? defaultValue;
        }

        public static Guid? GetHeaderId(this Headers headers, string key)
        {
            if (headers.TryGetHeader(key, out var value))
            {
                return value switch
                {
                    Guid guid => guid,
                    string text when Guid.TryParse(text, out var guid) => guid,
                    _ => default
                };
            }

            return default;
        }
    }
}
