namespace MassTransit.AzureTable.Saga
{
    using Microsoft.Azure.Cosmos.Table;


    public class AzureTableDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        public AzureTableDatabaseContext(CloudTable table, ISagaKeyFormatter<TSaga> keyFormatter)
        {
            Table = table;
            Formatter = keyFormatter;

            Converter = EntityConverterFactory.CreateConverter<TSaga>();
        }

        public ISagaKeyFormatter<TSaga> Formatter { get; }
        public CloudTable Table { get; }
        public IEntityConverter<TSaga> Converter { get; }
    }
}
