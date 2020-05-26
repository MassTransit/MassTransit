namespace MassTransit.EntityFrameworkCoreIntegration.Turnout
{
    using System.Collections.Generic;
    using Mappings;
    using MassTransit.Turnout.Components.StateMachines;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class TurnoutJobTypeStateSagaMap :
        SagaClassMap<TurnoutJobTypeState>
    {
        protected override void Configure(EntityTypeBuilder<TurnoutJobTypeState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);

            entity.Property(x => x.ActiveJobCount);
            entity.Property(x => x.ConcurrentJobLimit);

            entity.Property(x => x.OverrideJobLimit);
            entity.Property(x => x.OverrideLimitExpiration);

            entity.Property(x => x.ActiveJobs)
                .HasConversion(new JsonValueConverter<List<ActiveJob>>())
                .Metadata.SetValueComparer(new JsonValueComparer<List<ActiveJob>>());
        }
    }
}
