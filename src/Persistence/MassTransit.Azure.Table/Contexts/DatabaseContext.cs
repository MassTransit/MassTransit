namespace MassTransit.Azure.Table.Contexts
{
    using Mapping;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;
    using Saga;


    public interface DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        ISagaKeyFormatter<TSaga> Formatter { get; }

        CloudTable Table { get; }

        IEntityConverter<TSaga> Converter { get; }
    }
}
