namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SimpleSaga.DataAccess
{
    using Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    class SimpleSagaMap : SagaClassMap<MassTransit.Tests.Saga.SimpleSaga>
    {
        protected override void Configure(EntityTypeBuilder<MassTransit.Tests.Saga.SimpleSaga> entityTypeBuilder, ModelBuilder modelBuilder)
        {
            entityTypeBuilder.Property(x => x.Name).HasMaxLength(40);
            entityTypeBuilder.Property(x => x.Initiated);
            entityTypeBuilder.Property(x => x.Observed);
            entityTypeBuilder.Property(x => x.Completed);

            entityTypeBuilder.ToTable("EfCoreSimpleSagas");
        }
    }
}
