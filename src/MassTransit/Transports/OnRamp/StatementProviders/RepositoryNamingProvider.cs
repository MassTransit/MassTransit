namespace MassTransit.Transports.Outbox.StatementProviders
{
    public class RepositoryNamingProvider : IRepositoryNamingProvider
    {
        public RepositoryNamingProvider(string schema)
        {
            Schema = schema;
        }

        public string Schema { get; }

        public string GetLocksTableName()
        {
            return $"{Schema}.Locks";
        }

        public string GetMessagesTableName()
        {
            return $"{Schema}.Messages";
        }

        public string GetSweepersTableName()
        {
            return $"{Schema}.Sweepers";
        }
    }
}
