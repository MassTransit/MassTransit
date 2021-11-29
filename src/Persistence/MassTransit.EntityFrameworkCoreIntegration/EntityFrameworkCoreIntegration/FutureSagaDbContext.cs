namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;


    public class FutureSagaDbContext :
        SagaDbContext
    {
        public FutureSagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new FutureStateMap(false); }
        }
    }
}
