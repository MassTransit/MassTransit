namespace MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga.DataAccess
{
    using System.Collections.Generic;
    using Mappings;
    using Microsoft.EntityFrameworkCore;


    public class DeadlockSagaDbContext : SagaDbContext
    {
        public DeadlockSagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new DeadlockSagaMap(); }
        }
    }
}
