namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public class EntityFrameworkSagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<DbContext, TSaga>
        where TSaga : class, ISaga
    {
        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(DbContext session, ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            throw new NotImplementedException();
        }
    }
}
