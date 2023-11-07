namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class OptimisticFutureSagaDbContext :
        FutureSagaDbContext
    {
        public OptimisticFutureSagaDbContext(DbContextOptions<OptimisticFutureSagaDbContext> options)
            : base(new DbContextOptions<FutureSagaDbContext>(options.Extensions.ToDictionary(x => x.GetType(), x => x)))
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new FutureStateMap(true); }
        }
    }
}
