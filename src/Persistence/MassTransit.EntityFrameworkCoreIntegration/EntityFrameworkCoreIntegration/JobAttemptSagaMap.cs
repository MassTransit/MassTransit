namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class JobAttemptSagaMap :
        SagaClassMap<JobAttemptSaga>
    {
        readonly bool _optimistic;

        public JobAttemptSagaMap(bool optimistic)
        {
            _optimistic = optimistic;
        }

        protected override void Configure(EntityTypeBuilder<JobAttemptSaga> entity, ModelBuilder model)
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
