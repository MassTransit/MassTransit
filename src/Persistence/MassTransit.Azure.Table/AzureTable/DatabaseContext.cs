namespace MassTransit.AzureTable
{
    using Microsoft.Azure.Cosmos.Table;


    public interface DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        ISagaKeyFormatter<TSaga> Formatter { get; }

        CloudTable Table { get; }

        IEntityConverter<TSaga> Converter { get; }
    }
}
