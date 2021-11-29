namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;


    class ChoirStatePessimisticSagaDbContext : SagaDbContext
    {
        public ChoirStatePessimisticSagaDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new EntityFrameworkChoirStateMap(); }
        }


        class EntityFrameworkChoirStateMap :
            SagaClassMap<ChoirStatePessimistic>
        {
            protected override void Configure(EntityTypeConfiguration<ChoirStatePessimistic> entity, DbModelBuilder modelBuilder)
            {
                entity.ToTable("ChoirStatesPessimistic", "test");

                entity.Property(x => x.CurrentState);
                entity.Property(x => x.BassName);
                entity.Property(x => x.BaritoneName);
                entity.Property(x => x.TenorName);
                entity.Property(x => x.CountertenorName);

                entity.Property(x => x.Harmony);
            }
        }
    }
}
