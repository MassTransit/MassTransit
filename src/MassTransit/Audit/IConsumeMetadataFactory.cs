namespace MassTransit.Audit
{
    public interface IConsumeMetadataFactory
    {
        MessageAuditMetadata CreateAuditMetadata<T>(ConsumeContext<T> context)
            where T : class;
    }
}
