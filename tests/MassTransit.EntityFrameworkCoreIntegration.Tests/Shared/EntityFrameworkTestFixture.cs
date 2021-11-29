namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Shared
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using TestFramework;


    public class EntityFrameworkTestFixture<TTestDbParameters, TDbContext> :
        InMemoryTestFixture
        where TTestDbParameters : ITestDbParameters, new()
        where TDbContext : DbContext
    {
        protected readonly DbContextOptionsBuilder<TDbContext> DbContextOptionsBuilder;
        protected readonly ILockStatementProvider RawSqlLockStatements;
        TTestDbParameters _testDbParameters;

        public EntityFrameworkTestFixture()
        {
            _testDbParameters = new TTestDbParameters();
            DbContextOptionsBuilder = _testDbParameters.GetDbContextOptions<TDbContext>();
            RawSqlLockStatements = _testDbParameters.RawSqlLockStatements;
        }

        protected void ApplyBuilderOptions(IServiceProvider provider, DbContextOptionsBuilder<TDbContext> builder)
        {
            _testDbParameters.Apply(typeof(TDbContext), builder);
        }

        protected void ApplyBuilderOptions(DbContextOptionsBuilder builder)
        {
            _testDbParameters.Apply(typeof(TDbContext), builder);
        }
    }
}
