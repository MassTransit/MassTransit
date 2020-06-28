namespace MassTransit.EntityFrameworkCoreIntegration.JobService
{
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

            entity.Ignore(x => x.Version);

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
