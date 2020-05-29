namespace MassTransit.Audit.MetadataFactories
{
    using System.Linq;


    public class DefaultConsumeMetadataFactory :
        IConsumeMetadataFactory
    {
        MessageAuditMetadata IConsumeMetadataFactory.CreateAuditMetadata<T>(ConsumeContext<T> context)
        {
            return CreateMetadata(context, "Consume");
        }

        static MessageAuditMetadata CreateMetadata(ConsumeContext context, string contextType)
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
                InputAddress = context.ReceiveContext.InputAddress?.AbsoluteUri,
                Headers = context.Headers?.GetAll()?.ToDictionary(k => k.Key, v => v.Value.ToString())
            };
        }
    }
}
