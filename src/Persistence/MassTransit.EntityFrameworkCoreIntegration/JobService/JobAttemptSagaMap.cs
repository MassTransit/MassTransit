namespace MassTransit.EntityFrameworkCoreIntegration.JobService
{
    using Mappings;
    using MassTransit.JobService.Components.StateMachines;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class JobAttemptSagaMap :
        SagaClassMap<JobAttemptSaga>
    {
        protected override void Configure(EntityTypeBuilder<JobAttemptSaga> entity, ModelBuilder model)
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
