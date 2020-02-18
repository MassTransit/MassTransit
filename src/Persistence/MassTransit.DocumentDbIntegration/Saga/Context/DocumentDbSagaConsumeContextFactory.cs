namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using System.Threading.Tasks;
    using MassTransit.Saga;


    public class DocumentDbSagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>
        where TSaga : class, IVersionedSaga
    {
        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(DatabaseContext<TSaga> context, ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            var sagaConsumeContext = new DocumentDbSagaConsumeContext<TSaga, T>(context, consumeContext, instance, mode);

            return Task.FromResult<SagaConsumeContext<TSaga, T>>(sagaConsumeContext);
        }
    }
}
