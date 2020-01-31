namespace MassTransit.EntityFrameworkIntegration.Saga.Context
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using MassTransit.Saga;


    public class EntityFrameworkSagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<DbContext, TSaga>
        where TSaga : class, ISaga
    {
        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(DbContext context, ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            return Task.FromResult<SagaConsumeContext<TSaga, T>>(new EntityFrameworkSagaConsumeContext<TSaga, T>(context, consumeContext, instance, mode));
        }
    }
}
