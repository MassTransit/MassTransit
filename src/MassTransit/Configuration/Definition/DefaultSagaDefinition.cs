namespace MassTransit.Definition
{
    using Saga;


    public class DefaultSagaDefinition<TSaga> :
        SagaDefinition<TSaga>
        where TSaga : class, ISaga
    {
    }
}
