namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class JobTypeSagaMap :
        SagaClassMap<JobTypeSaga>
    {
        readonly bool _optimistic;

        public JobTypeSagaMap(bool optimistic)
        {
            _optimistic = optimistic;
        }

        protected override void Configure(EntityTypeBuilder<JobTypeSaga> entity, ModelBuilder model)
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

            entity.Property(x => x.ActiveJobCount);
            entity.Property(x => x.ConcurrentJobLimit);

            entity.Property(x => x.OverrideJobLimit);
            entity.Property(x => x.OverrideLimitExpiration);

            entity.Property(x => x.ActiveJobs)
                .HasJsonConversion();

            entity.Property(x => x.Instances)
                .HasJsonConversion();
        }
    }
}
