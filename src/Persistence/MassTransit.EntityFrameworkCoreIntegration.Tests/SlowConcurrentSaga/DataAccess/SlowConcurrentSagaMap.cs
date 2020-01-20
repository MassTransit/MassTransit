namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga.DataAccess
{
    using Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    class SlowConcurrentSagaMap : SagaClassMap<SlowConcurrentSaga>
    {
        protected override void Configure(EntityTypeBuilder<SlowConcurrentSaga> entityTypeBuilder, ModelBuilder modelBuilder)
        {
            entityTypeBuilder.Property(x => x.Name).HasMaxLength(40);
            entityTypeBuilder.Property(x => x.CurrentState).HasMaxLength(40);

            entityTypeBuilder.ToTable("EfCoreSlowConcurrentSagas");
        }
    }
}
