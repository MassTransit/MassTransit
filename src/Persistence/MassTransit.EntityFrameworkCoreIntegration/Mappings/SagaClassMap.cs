namespace MassTransit.EntityFrameworkCoreIntegration.Mappings
{
    using System;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public abstract class SagaClassMap<TSaga> :
        ISagaClassMap<TSaga>
        where TSaga : class, ISaga
    {
        public Type SagaType => typeof(TSaga);

        public virtual void Configure(ModelBuilder modelBuilder)
        {
            EntityTypeBuilder<TSaga> entityTypeBuilder = modelBuilder.Entity<TSaga>();

            entityTypeBuilder.HasKey(p => p.CorrelationId);

            entityTypeBuilder.Property(p => p.CorrelationId)
                .ValueGeneratedNever();

            Configure(entityTypeBuilder, modelBuilder);
        }

        protected virtual void Configure(EntityTypeBuilder<TSaga> entity, ModelBuilder model)
        {
        }
    }
}
