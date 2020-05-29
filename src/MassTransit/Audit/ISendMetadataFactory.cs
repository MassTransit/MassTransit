namespace MassTransit.Audit
{
    public interface ISendMetadataFactory
    {
        MessageAuditMetadata CreateAuditMetadata<T>(SendContext<T> context)
            where T : class;

        MessageAuditMetadata CreateAuditMetadata<T>(PublishContext<T> context)
            where T : class;
    }
}
