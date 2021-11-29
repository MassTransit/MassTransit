namespace MassTransit.Configuration
{
    public class DefaultSagaDefinition<TSaga> :
        SagaDefinition<TSaga>
        where TSaga : class, ISaga
    {
    }
}
