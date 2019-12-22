namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using TestFramework;

    public class EntityFrameworkTestFixture<TOptionsProvider, TDbContext>
        : InMemoryTestFixture
        where TOptionsProvider : ITestDbContextOptionsProvider, new()
        where TDbContext : DbContext
    {
        protected readonly DbContextOptionsBuilder DbContextOptionsBuilder;

        public EntityFrameworkTestFixture()
        {
            DbContextOptionsBuilder = new TOptionsProvider().GetDbContextOptions(typeof(TDbContext));
        }
    }
}
