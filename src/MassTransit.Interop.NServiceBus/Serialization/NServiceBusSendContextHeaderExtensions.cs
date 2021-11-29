namespace MassTransit.Serialization
{
    using System;
    using Metadata;
    using Transports;


    public static class NServiceBusSendContextHeaderExtensions
    {
        public static void SetNServiceBusHeaders<T>(this SendContext<T> context)
            where T : class
        {
            if (context.MessageId.HasValue)
                context.Headers.Set(NServiceBusMessageHeaders.MessageId, context.MessageId.Value.ToString());

            if (context.CorrelationId.HasValue)
                context.Headers.Set(NServiceBusMessageHeaders.CorrelationId, context.CorrelationId.Value.ToString());

            if (context.ConversationId.HasValue)
                context.Headers.Set(NServiceBusMessageHeaders.ConversationId, context.ConversationId.Value.ToString());

            if (context.SourceAddress != null)
                context.Headers.Set(NServiceBusMessageHeaders.OriginatingEndpoint, context.SourceAddress.GetEndpointName());

            if (context.ResponseAddress != null)
                context.Headers.Set(NServiceBusMessageHeaders.ReplyToAddress, context.ResponseAddress.GetEndpointName());

            context.Headers.Set(NServiceBusMessageHeaders.ContentType, NServiceBusJsonMessageSerializer.ContentTypeHeaderValue);
            context.Headers.Set(NServiceBusMessageHeaders.EnclosedMessageTypes, string.Join(";", NServiceBusTypeCache<T>.MessageTypeNames));
            context.Headers.Set(NServiceBusMessageHeaders.OriginatingMachine, HostMetadataCache.Host.MachineName);

            context.Headers.Set(NServiceBusMessageHeaders.TimeSent, (context.SentTime ?? DateTime.UtcNow).ToString("yyyy-MM-dd hh:mm:ss:ffffff Z"));
        }
    }
}
