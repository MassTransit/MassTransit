namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SagaWithDependency.DataAccess
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;


    public class SagaWithDependencyContext : SagaDbContext
    {
        public SagaWithDependencyContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SagaWithDependencyMap(); }
        }
    }
}
