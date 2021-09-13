namespace MassTransit.Serialization
{
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

            context.Headers.Set(MessageHeaders.MessageType, string.Join(";", TypeMetadataCache<T>.MessageTypeNames));

            if (context.ResponseAddress != null)
                context.Headers.Set(MessageHeaders.ResponseAddress, context.ResponseAddress);

            if (context.FaultAddress != null)
                context.Headers.Set(MessageHeaders.FaultAddress, context.FaultAddress);

            if (context.InitiatorId.HasValue)
                context.Headers.Set(MessageHeaders.InitiatorId, context.InitiatorId.Value.ToString());

            if (context.SourceAddress != null)
                context.Headers.Set(MessageHeaders.SourceAddress, context.SourceAddress);
        }
    }
}
