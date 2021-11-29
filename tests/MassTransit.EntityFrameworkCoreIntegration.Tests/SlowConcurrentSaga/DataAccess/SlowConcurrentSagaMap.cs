namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga.DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    class SlowConcurrentSagaMap : SagaClassMap<SlowConcurrentSaga>
    {
        protected override void Configure(EntityTypeBuilder<SlowConcurrentSaga> entity, ModelBuilder model)
        {
            entity.Property(x => x.Name).HasMaxLength(40);
            entity.Property(x => x.CurrentState).HasMaxLength(40);

            entity.ToTable("EfCoreSlowConcurrentSagas");
        }
    }
}
