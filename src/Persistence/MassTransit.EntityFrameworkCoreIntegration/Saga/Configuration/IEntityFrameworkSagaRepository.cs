namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Configuration
{
    using Mappings;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public interface IEntityFrameworkSagaRepository
    {
        void AddSagaClassMap<TSaga>(ISagaClassMap<TSaga> sagaClassMap)
            where TSaga : class, ISaga;

        DbContext GetDbContext();
    }
}
