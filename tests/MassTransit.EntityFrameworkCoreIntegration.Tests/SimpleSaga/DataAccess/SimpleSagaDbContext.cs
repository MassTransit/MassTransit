namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SimpleSaga.DataAccess
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;


    public class SimpleSagaDbContext : SagaDbContext
    {
        public SimpleSagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SimpleSagaMap(); }
        }
    }
}
