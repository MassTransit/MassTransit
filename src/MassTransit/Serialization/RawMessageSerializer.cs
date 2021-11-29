namespace MassTransit.Serialization
{
    using System.Text;
    using System.Text.Json;
    using Metadata;


    public abstract class RawMessageSerializer
    {
        protected virtual void SetRawMessageHeaders<T>(SendContext context)
            where T : class
        {
            if (context.MessageId.HasValue)
                context.Headers.Set(MessageHeaders.MessageId, context.MessageId.Value.ToString());

            if (context.CorrelationId.HasValue)
                context.Headers.Set(MessageHeaders.CorrelationId, context.CorrelationId.Value.ToString());

            if (context.ConversationId.HasValue)
                context.Headers.Set(MessageHeaders.ConversationId, context.ConversationId.Value.ToString());

            if (context.InitiatorId.HasValue)
                context.Headers.Set(MessageHeaders.InitiatorId, context.InitiatorId.Value.ToString());

            if (context.RequestId.HasValue)
                context.Headers.Set(MessageHeaders.RequestId, context.RequestId.Value.ToString());

            context.Headers.Set(MessageHeaders.MessageType, string.Join(";", MessageTypeCache<T>.MessageTypeNames));

            if (context.ResponseAddress != null)
                context.Headers.Set(MessageHeaders.ResponseAddress, context.ResponseAddress);

            if (context.FaultAddress != null)
                context.Headers.Set(MessageHeaders.FaultAddress, context.FaultAddress);

            if (context.SourceAddress != null)
                context.Headers.Set(MessageHeaders.SourceAddress, context.SourceAddress);

            context.Headers.Set(MessageHeaders.Host.Info, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(HostMetadataCache.Host)));
        }
    }
}
