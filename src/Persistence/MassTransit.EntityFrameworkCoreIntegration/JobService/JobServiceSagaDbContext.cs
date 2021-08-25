namespace MassTransit.EntityFrameworkCoreIntegration.JobService
{
    using System.Collections.Generic;
    using Mappings;
    using Microsoft.EntityFrameworkCore;


    public class JobServiceSagaDbContext :
        SagaDbContext
    {
        public JobServiceSagaDbContext(DbContextOptions<JobServiceSagaDbContext> options)
            : base(options)
        {
        }

        public JobServiceSagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new JobTypeSagaMap();
                yield return new JobSagaMap();
                yield return new JobAttemptSagaMap();
            }
        }
    }
}
