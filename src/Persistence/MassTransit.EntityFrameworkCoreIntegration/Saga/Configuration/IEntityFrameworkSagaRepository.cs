namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Configuration
{
    using System;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public interface IEntityFrameworkSagaRepository
    {
        void ConfigureSaga<TSaga>(Action<EntityTypeBuilder<TSaga>> configure = null)
            where TSaga : class, ISaga;

        SagaDbContext GetDbContext();
    }
}
