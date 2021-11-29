namespace MassTransit
{
    using EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;


    public interface IEntityFrameworkSagaRepository
    {
        void AddSagaClassMap<TSaga>(ISagaClassMap<TSaga> sagaClassMap)
            where TSaga : class, ISaga;

        DbContext GetDbContext();
    }
}
