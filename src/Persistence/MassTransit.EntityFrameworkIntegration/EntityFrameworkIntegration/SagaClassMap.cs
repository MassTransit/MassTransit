namespace MassTransit.EntityFrameworkIntegration
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;


    public abstract class SagaClassMap<TSaga> :
        ISagaClassMap
        where TSaga : class, ISaga
    {
        public Type SagaType => typeof(TSaga);

        public void Configure(DbModelBuilder modelBuilder)
        {
            EntityTypeConfiguration<TSaga> cfg = modelBuilder.Entity<TSaga>();

            cfg.HasKey(t => t.CorrelationId, x => x.IsClustered(true));

            cfg.Property(t => t.CorrelationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Configure(cfg, modelBuilder);
        }

        protected virtual void Configure(EntityTypeConfiguration<TSaga> entity, DbModelBuilder model)
        {
        }
    }
}
