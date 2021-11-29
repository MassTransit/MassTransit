namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class JobSagaMap :
        SagaClassMap<JobSaga>
    {
        readonly bool _optimistic;

        public JobSagaMap(bool optimistic)
        {
            _optimistic = optimistic;
        }

        protected override void Configure(EntityTypeBuilder<JobSaga> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);

            entity.Ignore(x => x.Version);

            if (_optimistic)
            {
                entity.Property(x => x.RowVersion)
                    .IsRowVersion();
            }
            else
                entity.Ignore(x => x.RowVersion);

            entity.Property(x => x.Submitted);
            entity.Property(x => x.ServiceAddress);
            entity.Property(x => x.JobTimeout);
            entity.Property(x => x.Job)
                .HasConversion(new JsonValueConverter<IDictionary<string, object>>())
                .Metadata.SetValueComparer(new JsonValueComparer<IDictionary<string, object>>());

            entity.Property(x => x.JobTypeId);

            entity.Property(x => x.AttemptId);
            entity.Property(x => x.RetryAttempt);

            entity.Property(x => x.Started);

            entity.Property(x => x.Completed);
            entity.Property(x => x.Duration);

            entity.Property(x => x.Faulted);
            entity.Property(x => x.Reason);

            entity.Property(x => x.JobSlotWaitToken);
            entity.Property(x => x.JobRetryDelayToken);
        }
    }
}
