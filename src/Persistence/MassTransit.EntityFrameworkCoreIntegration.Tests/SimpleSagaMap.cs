namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using Mappings;
    using MassTransit.Tests.Saga;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    class SimpleSagaMap : SagaClassMap<SimpleSaga>
    {
        protected override void Configure(EntityTypeBuilder<SimpleSaga> entityTypeBuilder, ModelBuilder modelBuilder)
        {
            entityTypeBuilder.Property(x => x.Name).HasMaxLength(40);
            entityTypeBuilder.Property(x => x.Initiated);
            entityTypeBuilder.Property(x => x.Observed);
            entityTypeBuilder.Property(x => x.Completed);

            entityTypeBuilder.ToTable("EfCoreSimpleSagas");
        }
    }
}
