namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
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
                .HasConversion(new JsonValueConverter<List<ActiveJob>>())
                .Metadata.SetValueComparer(new JsonValueComparer<List<ActiveJob>>());

            entity.Property(x => x.Instances)
                .HasConversion(new JsonValueConverter<Dictionary<Uri, JobTypeInstance>>())
                .Metadata.SetValueComparer(new JsonValueComparer<Dictionary<Uri, JobTypeInstance>>());
        }
    }
}
