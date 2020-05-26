namespace MassTransit.EntityFrameworkCoreIntegration.Turnout
{
    using Mappings;
    using MassTransit.Turnout.Components.StateMachines;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class TurnoutJobStateSagaMap :
        SagaClassMap<TurnoutJobState>
    {
        protected override void Configure(EntityTypeBuilder<TurnoutJobState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);

            entity.Property(x => x.Submitted);
            entity.Property(x => x.ServiceAddress);
            entity.Property(x => x.JobTimeout);
            entity.Property(x => x.JobJson);
            entity.Property(x => x.JobTypeId);

            entity.Property(x => x.AttemptId);

            entity.Property(x => x.Started);

            entity.Property(x => x.Completed);
            entity.Property(x => x.Duration);

            entity.Property(x => x.Faulted);
            entity.Property(x => x.Reason);

            entity.Property(x => x.StartJobRequestId);
            entity.HasIndex(x => x.StartJobRequestId).IsUnique().HasFilter(null);

            entity.Property(x => x.JobSlotRequestId);
            entity.HasIndex(x => x.JobSlotRequestId).IsUnique().HasFilter(null);

            entity.Property(x => x.JobSlotWaitToken);
        }
    }
}
