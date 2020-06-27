namespace MassTransit.Azure.Table.Audit
{
    public interface IPartitionKeyFormatter
    {
        string Format<T>(AuditRecord record)
            where T : class;
    }
}
