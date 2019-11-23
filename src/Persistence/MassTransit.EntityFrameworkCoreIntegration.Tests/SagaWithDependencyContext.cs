namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System.Collections.Generic;
    using Mappings;
    using Microsoft.EntityFrameworkCore;


    public class SagaWithDependencyContext : SagaDbContext
    {
        public SagaWithDependencyContext(DbContextOptions<SagaWithDependencyContext> options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SagaWithDependencyMap(); }
        }
    }
}
