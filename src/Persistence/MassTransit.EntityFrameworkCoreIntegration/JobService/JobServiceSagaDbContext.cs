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

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new JobTypeSagaMap(false);
                yield return new JobSagaMap(false);
                yield return new JobAttemptSagaMap(false);
            }
        }
    }


    public class OptimisticJobServiceSagaDbContext :
        SagaDbContext
    {
        public OptimisticJobServiceSagaDbContext(DbContextOptions<OptimisticJobServiceSagaDbContext> options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new JobTypeSagaMap(true);
                yield return new JobSagaMap(true);
                yield return new JobAttemptSagaMap(true);
            }
        }
    }
}
