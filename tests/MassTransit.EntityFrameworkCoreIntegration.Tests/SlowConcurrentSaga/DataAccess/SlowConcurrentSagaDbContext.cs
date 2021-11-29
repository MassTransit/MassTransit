namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga.DataAccess
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;


    public class SlowConcurrentSagaDbContext : SagaDbContext
    {
        public SlowConcurrentSagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SlowConcurrentSagaMap(); }
        }
    }
}
