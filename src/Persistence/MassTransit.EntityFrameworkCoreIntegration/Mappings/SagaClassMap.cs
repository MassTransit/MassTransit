namespace MassTransit.EntityFrameworkCoreIntegration.Mappings
{
    using System;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public abstract class SagaClassMap<TSaga> : ISagaClassMap<TSaga>
        where TSaga : class, ISaga
    {
        public Type SagaType => typeof(TSaga);

        public virtual void Configure(ModelBuilder modelBuilder)
        {
            if (!TypeCache<TSaga>.ReadWritePropertyCache.TryGetProperty("CorrelationId", out ReadWriteProperty<TSaga> _))
                throw new ConfigurationException("The CorrelationId property must be read/write for use with Entity Framework. Add a setter to the property.");
            EntityTypeBuilder<TSaga> cfg = modelBuilder.Entity<TSaga>();

            cfg.HasKey(t => t.CorrelationId);

            cfg.Property(t => t.CorrelationId)
                .ValueGeneratedNever();

            Configure(cfg, modelBuilder);
        }

        protected virtual void Configure(EntityTypeBuilder<TSaga> cfg, ModelBuilder modelBuilder)
        {
        }
    }
}
