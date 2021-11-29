#nullable enable
namespace MassTransit
{
    using System;
    using System.Text;


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

        /// <summary>
        /// Returns the SourceAddress from the transport headers, if present
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Uri? GetSourceAddress(this ReceiveContext context)
        {
            return context.TransportHeaders.GetEndpointAddress(MessageHeaders.SourceAddress);
        }

        /// <summary>
        /// Returns the ResponseAddress from the transport headers, if present
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Uri? GetResponseAddress(this ReceiveContext context)
        {
            return context.TransportHeaders.GetEndpointAddress(MessageHeaders.ResponseAddress);
        }

        /// <summary>
        /// Returns the FaultAddress from the transport headers, if present
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Uri? GetFaultAddress(this ReceiveContext context)
        {
            return context.TransportHeaders.GetEndpointAddress(MessageHeaders.FaultAddress);
        }

        public static string[] GetMessageTypes(this ReceiveContext context)
        {
            if (context.TransportHeaders.TryGetHeader(MessageHeaders.MessageType, out var value) && value is string text && !string.IsNullOrWhiteSpace(text))
                return text.Split(';');

            return Array.Empty<string>();
        }

        /// <summary>
        /// Returns either the Content-Encoding from the transport header, or the default UTF-8 encoding (no BOM).
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Encoding GetMessageEncoding(this ReceiveContext context)
        {
            if (context.TransportHeaders.TryGetHeader("Content-Encoding", out var value) && value is string text && !string.IsNullOrWhiteSpace(text))
                return Encoding.GetEncoding(text);

            return MessageDefaults.Encoding;
        }

        /// <summary>
        /// Returns the messageId from the transport header, if available
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Guid? GetMessageId(this Headers headers)
        {
            return headers.GetHeaderId(MessageHeaders.MessageId);
        }

        /// <summary>
        /// Returns the messageId from the transport header, if available
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Guid GetMessageId(this Headers headers, Guid defaultValue)
        {
            return headers.GetHeaderId(MessageHeaders.MessageId, defaultValue);
        }

        /// <summary>
        /// Returns the CorrelationId from the transport header, if available
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Guid? GetCorrelationId(this Headers headers)
        {
            return headers.GetHeaderId(MessageHeaders.CorrelationId);
        }

        /// <summary>
        /// Returns the ConversationId from the transport header, if available
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Guid? GetConversationId(this Headers headers)
        {
            return headers.GetHeaderId(MessageHeaders.ConversationId);
        }

        /// <summary>
        /// Returns the RequestId from the transport header, if available
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Guid? GetRequestId(this Headers headers)
        {
            return headers.GetHeaderId(MessageHeaders.RequestId);
        }

        /// <summary>
        /// Returns the InitiatorId from the transport header, if available
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Guid? GetInitiatorId(this Headers headers)
        {
            return headers.GetHeaderId(MessageHeaders.InitiatorId);
        }

        /// <summary>
        /// Returns the SourceAddress from the transport headers, if present
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Uri? GetSourceAddress(this Headers headers)
        {
            return headers.GetEndpointAddress(MessageHeaders.SourceAddress);
        }

        /// <summary>
        /// Returns the ResponseAddress from the transport headers, if present
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Uri? GetResponseAddress(this Headers headers)
        {
            return headers.GetEndpointAddress(MessageHeaders.ResponseAddress);
        }

        /// <summary>
        /// Returns the FaultAddress from the transport headers, if present
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Uri? GetFaultAddress(this Headers headers)
        {
            return headers.GetEndpointAddress(MessageHeaders.FaultAddress);
        }

        public static string[] GetMessageTypes(this Headers headers)
        {
            if (headers.TryGetHeader(MessageHeaders.MessageType, out var value))
            {
                if (value is string text && !string.IsNullOrWhiteSpace(text))
                    return text.Split(';');
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// Returns either the Content-Encoding from the transport header, or the default UTF-8 encoding (no BOM).
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Encoding GetMessageEncoding(this Headers headers)
        {
            if (headers.TryGetHeader("Content-Encoding", out var value) && value is string text && !string.IsNullOrWhiteSpace(text))
                return Encoding.GetEncoding(text);

            return MessageDefaults.Encoding;
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

        public static Uri? GetEndpointAddress(this Headers headers, string key)
        {
            if (headers.TryGetHeader(key, out var value))
            {
                try
                {
                    return value switch
                    {
                        Uri address => address,
                        string text when !string.IsNullOrWhiteSpace(text) => new Uri(text, UriKind.Absolute),
                        _ => default
                    };
                }
                catch (FormatException)
                {
                    return default;
                }
            }

            return default;
        }
    }
}
