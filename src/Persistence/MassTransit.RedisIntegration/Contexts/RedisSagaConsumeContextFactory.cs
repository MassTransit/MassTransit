namespace MassTransit.RedisIntegration.Contexts
{
    using System.Threading.Tasks;
    using Saga;


    public class RedisSagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>
        where TSaga : class, IVersionedSaga
    {
        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(DatabaseContext<TSaga> context, ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            return Task.FromResult<SagaConsumeContext<TSaga, T>>(new RedisSagaConsumeContext<TSaga, T>(context, consumeContext, instance, mode));
        }
    }
}
