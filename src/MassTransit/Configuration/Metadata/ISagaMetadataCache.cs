namespace MassTransit.Metadata
{
    using Saga;


    public interface ISagaMetadataCache<out TSaga>
        where TSaga : class, ISaga
    {
        SagaInterfaceType[] InitiatedByTypes { get; }
        SagaInterfaceType[] OrchestratesTypes { get; }
        SagaInterfaceType[] ObservesTypes { get; }

        SagaInstanceFactoryMethod<TSaga> FactoryMethod { get; }
    }
}
