namespace MassTransit.EntityFrameworkCoreIntegration.Turnout
{
    using Mappings;
    using MassTransit.Turnout.Components.StateMachines;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class TurnoutJobAttemptStateSagaMap :
        SagaClassMap<TurnoutJobAttemptState>
    {
        protected override void Configure(EntityTypeBuilder<TurnoutJobAttemptState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);

            entity.Property(x => x.JobId);
            entity.Property(x => x.RetryAttempt);

            entity.HasIndex(x => new
            {
                x.JobId,
                x.RetryAttempt
            }).IsUnique().HasFilter(null);

            entity.Property(x => x.ServiceAddress);

            entity.Property(x => x.InstanceAddress);

            entity.Property(x => x.Started);

            entity.Property(x => x.Faulted);

            entity.Property(x => x.StatusCheckTokenId);
        }
    }
}
