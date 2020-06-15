namespace MassTransit.EntityFrameworkCoreIntegration.JobService
{
    using System.Collections.Generic;
    using Mappings;
    using MassTransit.JobService.Components.StateMachines;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class JobSagaMap :
        SagaClassMap<JobSaga>
    {
        protected override void Configure(EntityTypeBuilder<JobSaga> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);

            entity.Property(x => x.Submitted);
            entity.Property(x => x.ServiceAddress);
            entity.Property(x => x.JobTimeout);
            entity.Property(x => x.Job)
                .HasConversion(new JsonValueConverter<IDictionary<string, object>>())
                .Metadata.SetValueComparer(new JsonValueComparer<IDictionary<string, object>>());

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
