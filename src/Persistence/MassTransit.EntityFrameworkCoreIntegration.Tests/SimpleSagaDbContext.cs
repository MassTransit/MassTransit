namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Mappings;
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
