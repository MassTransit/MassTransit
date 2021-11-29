namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Collections.Generic;
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
}
