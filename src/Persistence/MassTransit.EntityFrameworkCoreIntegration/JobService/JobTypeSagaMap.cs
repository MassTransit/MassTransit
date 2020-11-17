namespace MassTransit.EntityFrameworkCoreIntegration.JobService
{
    using System;
    using System.Collections.Generic;
    using Mappings;
    using MassTransit.JobService.Components.StateMachines;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class JobTypeSagaMap :
        SagaClassMap<JobTypeSaga>
    {
        protected override void Configure(EntityTypeBuilder<JobTypeSaga> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);

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
