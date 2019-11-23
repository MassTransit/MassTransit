namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;
    using Mappings;


    class ChoirStatePessimisticSagaDbContext : SagaDbContext
    {
        class EntityFrameworkChoirStateMap :
            SagaClassMap<ChoirStatePessimistic>
        {
            protected override void Configure(EntityTypeConfiguration<ChoirStatePessimistic> cfg, DbModelBuilder modelBuilder)
            {
                cfg.ToTable("ChoirStatesPessimistic", "test");

                cfg.Property(x => x.CurrentState);
                cfg.Property(x => x.BassName);
                cfg.Property(x => x.BaritoneName);
                cfg.Property(x => x.TenorName);
                cfg.Property(x => x.CountertenorName);

                cfg.Property(x => x.Harmony);
            }
        }


        public ChoirStatePessimisticSagaDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new EntityFrameworkChoirStateMap(); }
        }
    }
}
