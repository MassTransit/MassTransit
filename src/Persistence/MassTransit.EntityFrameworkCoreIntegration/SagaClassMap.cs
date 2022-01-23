namespace MassTransit
{
    using System;
    using EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public abstract class SagaClassMap<TSaga> :
        ISagaClassMap<TSaga>
        where TSaga : class, ISaga
    {
        public Type SagaType => typeof(TSaga);

        public virtual void Configure(ModelBuilder model)
        {
            EntityTypeBuilder<TSaga> entity = model.Entity<TSaga>();

            var key = entity.HasKey(p => p.CorrelationId);

            entity.Property(p => p.CorrelationId)
                .ValueGeneratedNever();

            Configure(entity, model);
        }

        protected virtual void Configure(EntityTypeBuilder<TSaga> entity, ModelBuilder model)
        {
        }

        /// <summary>
        /// Override to configure the primary CorrelationId key, to add things like clustering
        /// </summary>
        /// <param name="keyBuilder"></param>
        /// <returns></returns>
        protected virtual KeyBuilder ConfigureCorrelationIdKey(KeyBuilder keyBuilder)
        {
            return keyBuilder;
        }
    }
}
