namespace MassTransit.EntityFrameworkCoreIntegration.JobService
{
    using System.Collections.Generic;
    using Mappings;
    using Microsoft.EntityFrameworkCore;


    public class JobServiceSagaDbContext :
        SagaDbContext
    {
        public JobServiceSagaDbContext(DbContextOptions options)
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
        JobServiceSagaDbContext
    {
        public OptimisticJobServiceSagaDbContext(DbContextOptions options)
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
