namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using Microsoft.EntityFrameworkCore;

    public class SimpleSagaDbContextWithResilienceStrategy : SimpleSagaDbContext
    {
        public SimpleSagaDbContextWithResilienceStrategy(DbContextOptions options)
            : base(options)
        {

        }
    }
}
