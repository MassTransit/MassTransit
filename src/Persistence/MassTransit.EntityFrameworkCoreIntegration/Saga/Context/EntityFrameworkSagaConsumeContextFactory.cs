namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Context
{
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


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
