namespace MassTransit.AzureTable
{
    using Azure.Data.Tables;


    public interface DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        ISagaKeyFormatter<TSaga> Formatter { get; }

        TableClient Table { get; }

        IEntityConverter<TSaga> Converter { get; }
    }
}
