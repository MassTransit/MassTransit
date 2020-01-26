namespace MassTransit.DapperIntegration.Context
{
    using System.Threading.Tasks;
    using Saga;


    public class DapperSagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>
        where TSaga : class, ISaga
    {
        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(DatabaseContext<TSaga> context, ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            var sagaConsumeContext = new DapperSagaConsumeContext<TSaga, T>(context, consumeContext, instance, mode);

            return Task.FromResult<SagaConsumeContext<TSaga, T>>(sagaConsumeContext);
        }
    }
}
