namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    // todo: should become obsolete once ef core 2.0 is released
    public interface IEntityTypeConfiguration<T> where T : class
    {
        void Configure(EntityTypeBuilder<T> entityTypeBuilder);
    }
}