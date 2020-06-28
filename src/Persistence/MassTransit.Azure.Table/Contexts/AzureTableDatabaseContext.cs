namespace MassTransit.Azure.Table.Contexts
{
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;
    using Saga;


    public class AzureTableDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        public AzureTableDatabaseContext(CloudTable table, ISagaKeyFormatter<TSaga> keyFormatter)
        {
            Table = table;
            Formatter = keyFormatter;
        }

        public ISagaKeyFormatter<TSaga> Formatter { get; }
        public CloudTable Table { get; }
    }
}
