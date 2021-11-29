namespace MassTransit.Configuration
{
    using Saga;


    public interface ISagaMetadataCache<out TSaga>
        where TSaga : class, ISaga
    {
        SagaInterfaceType[] InitiatedByTypes { get; }
        SagaInterfaceType[] OrchestratesTypes { get; }
        SagaInterfaceType[] ObservesTypes { get; }
        SagaInterfaceType[] InitiatedByOrOrchestratesTypes { get; }

        SagaInstanceFactoryMethod<TSaga> FactoryMethod { get; }
    }
}
