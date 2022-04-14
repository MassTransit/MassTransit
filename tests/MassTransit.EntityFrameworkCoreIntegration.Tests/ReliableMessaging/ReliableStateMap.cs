namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using MassTransit.Tests.ReliableMessaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class ReliableStateMap :
        SagaClassMap<ReliableState>
    {
        protected override void Configure(EntityTypeBuilder<ReliableState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);
        }
    }
}
