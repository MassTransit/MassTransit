namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using MassTransit.Tests.Saga;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SimpleSagaMap : IEntityTypeConfiguration<SimpleSaga>
    {
        public void Configure(EntityTypeBuilder<SimpleSaga> entityTypeBuilder)
        {
            entityTypeBuilder.Property(x => x.Name).HasMaxLength(40);
            entityTypeBuilder.Property(x => x.Initiated);
            entityTypeBuilder.Property(x => x.Observed);
            entityTypeBuilder.Property(x => x.Completed);

            entityTypeBuilder.ToTable("EfCoreSimpleSagas");
        }
    }
}
