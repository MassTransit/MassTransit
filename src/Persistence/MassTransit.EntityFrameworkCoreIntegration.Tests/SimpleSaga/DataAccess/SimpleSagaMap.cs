namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SimpleSaga.DataAccess
{
    using Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    class SimpleSagaMap : SagaClassMap<MassTransit.Tests.Saga.SimpleSaga>
    {
        protected override void Configure(EntityTypeBuilder<MassTransit.Tests.Saga.SimpleSaga> entity, ModelBuilder model)
        {
            entity.Property(x => x.Name).HasMaxLength(40);
            entity.Property(x => x.Initiated);
            entity.Property(x => x.Observed);
            entity.Property(x => x.Completed);

            entity.ToTable("EfCoreSimpleSagas");
        }
    }
}
