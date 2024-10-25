namespace MassTransit.AzureTable.Saga
{
    using Azure.Data.Tables;


    public class AzureTableDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        public AzureTableDatabaseContext(TableClient table, ISagaKeyFormatter<TSaga> keyFormatter)
        {
            Table = table;
            Formatter = keyFormatter;

            Converter = EntityConverterFactory.CreateConverter<TSaga>();
        }

        public ISagaKeyFormatter<TSaga> Formatter { get; }
        public TableClient Table { get; }
        public IEntityConverter<TSaga> Converter { get; }
    }
}
