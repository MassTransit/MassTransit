namespace MassTransit.EntityFrameworkIntegration.Mappings
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;
    using MassTransit.Saga;


    public abstract class SagaClassMap<TSaga> : ISagaClassMap
        where TSaga : class, ISaga
    {
        public Type SagaType => typeof(TSaga);

        public void Configure(DbModelBuilder modelBuilder)
        {
            if (!TypeCache<TSaga>.ReadWritePropertyCache.TryGetProperty("CorrelationId", out ReadWriteProperty<TSaga> _))
                throw new ConfigurationException("The CorrelationId property must be read/write for use with Entity Framework. Add a setter to the property.");

            EntityTypeConfiguration<TSaga> cfg = modelBuilder.Entity<TSaga>();

            cfg.HasKey(t => t.CorrelationId);

            cfg.Property(t => t.CorrelationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Configure(cfg, modelBuilder);
        }

        protected virtual void Configure(EntityTypeConfiguration<TSaga> cfg, DbModelBuilder modelBuilder)
        {
        }
    }
}
