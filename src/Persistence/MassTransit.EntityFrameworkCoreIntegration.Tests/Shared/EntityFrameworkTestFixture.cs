namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using TestFramework;

    public class EntityFrameworkTestFixture<TTestDbParameters, TDbContext>
        : InMemoryTestFixture
        where TTestDbParameters : ITestDbParameters, new()
        where TDbContext : DbContext
    {
        protected readonly DbContextOptionsBuilder DbContextOptionsBuilder;
        protected readonly IRawSqlLockStatements RawSqlLockStatements;

        public EntityFrameworkTestFixture()
        {
            var testDbParameters = new TTestDbParameters();
            DbContextOptionsBuilder = testDbParameters.GetDbContextOptions(typeof(TDbContext));
            RawSqlLockStatements = testDbParameters.RawSqlLockStatements;
        }
    }
}
