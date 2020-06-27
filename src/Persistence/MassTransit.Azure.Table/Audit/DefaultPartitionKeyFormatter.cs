namespace MassTransit.Azure.Table.Audit
{
    public class DefaultPartitionKeyFormatter :
        IPartitionKeyFormatter
    {
        public string Format<T>(AuditRecord record)
            where T : class
        {
            return record.ContextType;
        }
    }
}
