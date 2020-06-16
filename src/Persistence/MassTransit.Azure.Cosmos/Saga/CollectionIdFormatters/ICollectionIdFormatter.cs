namespace MassTransit.Azure.Cosmos.Saga.CollectionIdFormatters
{
    using MassTransit.Saga;


    public interface ICollectionIdFormatter
    {
        string Saga<TSaga>()
            where TSaga : ISaga;
    }
}
