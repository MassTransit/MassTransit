namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;
    using Mappings;


    class ChoirStateOptimisticSagaDbContext : SagaDbContext
    {
        class EntityFrameworkChoirStateMap :
            SagaClassMap<ChoirStateOptimistic>
        {
            protected override void Configure(EntityTypeConfiguration<ChoirStateOptimistic> cfg, DbModelBuilder modelBuilder)
            {
                cfg.Property(x => x.RowVersion)
                    .IsRowVersion();

                cfg.Property(x => x.CurrentState);
                cfg.Property(x => x.BassName);
                cfg.Property(x => x.BaritoneName);
                cfg.Property(x => x.TenorName);
                cfg.Property(x => x.CountertenorName);

                cfg.Property(x => x.Harmony);
            }
        }

        public ChoirStateOptimisticSagaDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new EntityFrameworkChoirStateMap(); }
        }
    }
}
