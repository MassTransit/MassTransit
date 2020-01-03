namespace MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga.DataAccess
{
    using Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    class DeadlockSagaMap : SagaClassMap<DeadlockSaga>
    {
        protected override void Configure(EntityTypeBuilder<DeadlockSaga> entityTypeBuilder, ModelBuilder modelBuilder)
        {
            entityTypeBuilder.Property(x => x.Name).HasMaxLength(40);
            entityTypeBuilder.Property(x => x.CurrentState).HasMaxLength(40);

            entityTypeBuilder.ToTable("EfCoreSlowConcurrentSagas");
        }
    }
}
