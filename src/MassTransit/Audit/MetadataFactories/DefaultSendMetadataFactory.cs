namespace MassTransit.Audit.MetadataFactories
{
    using System.Linq;


    public class DefaultSendMetadataFactory :
        ISendMetadataFactory
    {
        MessageAuditMetadata ISendMetadataFactory.CreateAuditMetadata<T>(SendContext<T> context)
        {
            return CreateMetadata(context, "Send");
        }

        MessageAuditMetadata ISendMetadataFactory.CreateAuditMetadata<T>(PublishContext<T> context)
        {
            return CreateMetadata(context, "Publish");
        }

        static MessageAuditMetadata CreateMetadata(SendContext context, string contextType)
        {
            return new MessageAuditMetadata
            {
                ContextType = contextType,
                ConversationId = context.ConversationId,
                CorrelationId = context.CorrelationId,
                InitiatorId = context.InitiatorId,
                MessageId = context.MessageId,
                RequestId = context.RequestId,
                SentTime = context.SentTime,
                DestinationAddress = context.DestinationAddress?.AbsoluteUri,
                SourceAddress = context.SourceAddress?.AbsoluteUri,
                FaultAddress = context.FaultAddress?.AbsoluteUri,
                ResponseAddress = context.ResponseAddress?.AbsoluteUri,
                InputAddress = "",
                Headers = context.Headers?.GetAll()?.ToDictionary(k => k.Key, v => v.Value.ToString())
            };
        }
    }
}
