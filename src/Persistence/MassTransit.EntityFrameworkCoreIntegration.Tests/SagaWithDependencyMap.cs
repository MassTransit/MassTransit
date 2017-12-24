namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SagaWithDependencyMap : IEntityTypeConfiguration<SagaWithDependency>
    {
        public void Configure(EntityTypeBuilder<SagaWithDependency> entityTypeBuilder)
        {
            entityTypeBuilder.Property(x => x.Name).HasMaxLength(40);
            entityTypeBuilder.Property(x => x.Initiated);
            entityTypeBuilder.Property(x => x.Completed);
            entityTypeBuilder.ToTable("EfCoreSagasWithDepencies");

            entityTypeBuilder.HasOne(x => x.Dependency).WithMany().IsRequired();
        }
    }
}