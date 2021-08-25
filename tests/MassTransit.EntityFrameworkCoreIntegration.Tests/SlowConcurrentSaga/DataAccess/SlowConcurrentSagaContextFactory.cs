namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga.DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Shared;


    public class SlowConcurrentSagaContextFactory : IDesignTimeDbContextFactory<SlowConcurrentSagaDbContext>
    {
        public SlowConcurrentSagaDbContext CreateDbContext(string[] args)
        {
            // used only for database update and migrations. Since IDesignTimeDbContextFactory is icky,
            // we only support command line tools for SQL Server, so use SQL Server if you need to do
            // migrations.

            var optionsBuilder = new SqlServerTestDbParameters().GetDbContextOptions<SlowConcurrentSagaDbContext>();

            return new SlowConcurrentSagaDbContext(optionsBuilder.Options);
        }

        public SlowConcurrentSagaDbContext CreateDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            return new SlowConcurrentSagaDbContext(optionsBuilder.Options);
        }
    }
}
