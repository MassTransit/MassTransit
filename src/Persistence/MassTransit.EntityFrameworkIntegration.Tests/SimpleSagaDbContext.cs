namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;
    using Mappings;
    using MassTransit.Tests.Saga;


    public class SimpleSagaDbContext : SagaDbContext
    {
        class SimpleSagaMap :
            SagaClassMap<SimpleSaga>
        {
            protected override void Configure(EntityTypeConfiguration<SimpleSaga> cfg, DbModelBuilder modelBuilder)
            {
                cfg.Property(x => x.Name)
                    .HasMaxLength(40);
                cfg.Property(x => x.Initiated);
                cfg.Property(x => x.Observed);
                cfg.Property(x => x.Completed);
            }
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SimpleSagaMap(); }
        }

        public SimpleSagaDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }
    }
}
