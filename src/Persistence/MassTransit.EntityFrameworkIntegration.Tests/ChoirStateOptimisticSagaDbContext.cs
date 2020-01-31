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
            protected override void Configure(EntityTypeConfiguration<ChoirStateOptimistic> entity, DbModelBuilder modelBuilder)
            {
                entity.Property(x => x.RowVersion)
                    .IsRowVersion();

                entity.Property(x => x.CurrentState);
                entity.Property(x => x.BassName);
                entity.Property(x => x.BaritoneName);
                entity.Property(x => x.TenorName);
                entity.Property(x => x.CountertenorName);

                entity.Property(x => x.Harmony);
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
