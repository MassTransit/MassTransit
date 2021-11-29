namespace MassTransit
{
    public interface ICosmosCollectionIdFormatter
    {
        string Saga<TSaga>()
            where TSaga : ISaga;
    }
}
